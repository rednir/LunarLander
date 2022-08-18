using Godot;
using System;

namespace LunarLander
{
    public class Game : Node
    {
        private const int FUEL_PENDING_SPEED = 100;

        private const int ALTITUDE_MULTIPLIER = 10;

        private readonly GameLogic Logic = new GameLogic();

        private float RocketPosition => -Logic.Altitude * ALTITUDE_MULTIPLIER;

        private bool IsGameWin => Logic.Velocity <= 10;

        private TextureRect Rocket;

        private Label PendingFuelLabel;

        private AnimationPlayer RocketAnimationPlayer;

        private AnimationPlayer HUDAnimationPlayer;

        private ProgressBar AltitudeProgressBar;

        private Label AltitudeLabel;

        private bool isHolding;

        private float pendingFuel;

        public override void _Ready()
        {
            Rocket = GetNode<TextureRect>("Rocket");
            PendingFuelLabel = GetNode<Label>("Rocket/PendingFuelLabel");
            RocketAnimationPlayer = GetNode<AnimationPlayer>("Rocket/AnimationPlayer");
            HUDAnimationPlayer = GetNode<AnimationPlayer>("HUD/AnimationPlayer");
            AltitudeProgressBar = GetNode<ProgressBar>("HUD/Altitude/ProgressBar");
            AltitudeLabel = GetNode<Label>("HUD/Altitude/Label");

            AltitudeProgressBar.MaxValue = 1000;
            Rocket.RectPosition = new Vector2(Rocket.RectPosition.x, RocketPosition);
        }

        public override void _Process(float delta)
        {
            UpdateHud();

            if (Logic.Altitude <= 0 && Rocket.RectPosition.y >= 0)
            {
                EndGame();
                return;
            }

            if (isHolding && pendingFuel < Logic.FuelRemaining)
            {
                // Make it easier to use zero fuel.
                if (pendingFuel < 0.5)
                    pendingFuel += delta * 5;
                else
                    pendingFuel += delta * FUEL_PENDING_SPEED;
            }
        }

        public override void _Input(InputEvent inputEvent)
        {
            if (inputEvent.IsActionPressed("restart"))
                GetTree().ReloadCurrentScene();

            if (Logic.Altitude <= 0)
                return;

            if (inputEvent is InputEventScreenTouch touchEvent)
            {
                if (touchEvent.Pressed)
                {
                    RocketAnimationPlayer.Play("holding");
                    isHolding = true;
                }
                else
                {
                    if (pendingFuel > 1)
                        RocketAnimationPlayer.Play("boost");
                    else
                        RocketAnimationPlayer.Play("boost-no-fuel");

                    isHolding = false;

                    Logic.Burn((int)pendingFuel);
                    GetTree().CreateTween()
                        .TweenProperty(Rocket, "rect_position", new Vector2(Rocket.RectPosition.x, RocketPosition), 0.6f)
                        .SetTrans(Tween.TransitionType.Quart)
                        .SetEase(Tween.EaseType.In);
                    pendingFuel = 0;
                }
            }
        }

        private void UpdateHud()
        {
            AltitudeProgressBar.Value = Logic.Altitude;
            AltitudeLabel.Text = $"{(int)Logic.Altitude}m";

            PendingFuelLabel.Text = pendingFuel > 1 ? $"-{(int)pendingFuel}" : string.Empty;

            // TODO: should be removed or hidden
            GetNode<Label>("HUD/Info").Text = $"SecondsPassed:{Logic.SecondsPassed}\nVelocity:{Logic.Velocity}\nFuelRemaining:{Logic.FuelRemaining}\npendingFuel:{pendingFuel}";
        }

        private void EndGame()
        {
            SetProcess(false);

            if (IsGameWin)
            {
                HUDAnimationPlayer.Play("win");
            }
            else
            {
                HUDAnimationPlayer.Play("lose");
                RocketAnimationPlayer.Play("explode");
            }

            HUDAnimationPlayer.Connect("animation_finished", this, "OnGameOverAnimationFinished");
        }

        public void OnGameOverAnimationFinished(string _)
        {
            GetTree().ReloadCurrentScene();
        }
    }
}