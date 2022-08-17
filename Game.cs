using Godot;
using System;

namespace LunarLander
{
    public class Game : Node
    {
        private const int FUEL_PENDING_SPEED = 50;

        private const int ALTITUDE_MULTIPLIER = 10;

        private readonly GameLogic Logic = new GameLogic();

        private float RocketPosition => -Logic.Altitude * ALTITUDE_MULTIPLIER;
        
        private bool IsGameOver => Logic.Altitude <= 0;

        private TextureRect Rocket;

        private Sprite Moon;

        private AnimationPlayer RocketAnimationPlayer;

        private ProgressBar AltitudeProgressBar;

        private Label AltitudeLabel;

        private bool isHolding;

        private float pendingFuel;

        public override void _Ready()
        {
            Rocket = GetNode<TextureRect>("Rocket");
            Moon = GetNode<Sprite>("Moon");
            RocketAnimationPlayer = GetNode<AnimationPlayer>("Rocket/AnimationPlayer");
            AltitudeProgressBar = GetNode<ProgressBar>("HUD/Altitude/ProgressBar");
            AltitudeLabel = GetNode<Label>("HUD/Altitude/Label");

            AltitudeProgressBar.MaxValue = 1000;
            Rocket.RectPosition = new Vector2(Rocket.RectPosition.x, RocketPosition);
        }

        public override void _Process(float delta)
        {
            if (IsGameOver)
            {
                EndGame();
                SetProcess(false);
                return;
            }

            if (isHolding)
                pendingFuel += delta * FUEL_PENDING_SPEED;

            UpdateHud();
        }

        public override void _Input(InputEvent inputEvent)
        {
            if (inputEvent.IsActionPressed("restart"))
                GetTree().ReloadCurrentScene();

            if (IsGameOver)
                return;

            if (inputEvent is InputEventScreenTouch touchEvent)
            {
                if (touchEvent.Pressed && pendingFuel < Logic.FuelRemaining)
                {
                    RocketAnimationPlayer.Play("holding");
                    isHolding = true;
                }
                else if (pendingFuel > 0)
                {
                    RocketAnimationPlayer.Play("boost");
                    isHolding = false;

                    Logic.Burn((int)pendingFuel);
                    GetTree().CreateTween()
                        .TweenProperty(Rocket, "rect_position", new Vector2(Rocket.RectPosition.x, RocketPosition), 0.75f)
                        .SetTrans(Tween.TransitionType.Quart)
                        .SetEase(Tween.EaseType.In);
                    pendingFuel = 0;
                }
            }
        }

        private void UpdateHud()
        {
            AltitudeProgressBar.Value = -Rocket.RectPosition.y / ALTITUDE_MULTIPLIER;
            AltitudeLabel.Text = $"{(int)Logic.Altitude}m";

            // TODO: should be removed or hidden
            GetNode<Label>("HUD/Info").Text = $"Velocity:{Logic.Velocity}\nFuelRemaining:{Logic.FuelRemaining}\npendingFuel:{pendingFuel}";
        }

        private void EndGame()
        {

        }
    }
}