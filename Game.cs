using Godot;
using System;

namespace LunarLander
{
    public class Game : Node
    {
        private const int FUEL_PENDING_SPEED = 50;

        private readonly GameLogic Logic = new GameLogic();

        private TextureRect Rocket;

        private AnimationPlayer RocketAnimationPlayer;

        private ProgressBar AltitudeProgressBar;

        private Label AltitudeLabel;

        private bool isHolding;

        private float pendingFuel;

        public override void _Ready()
        {
            Rocket = GetNode<TextureRect>("Rocket");
            RocketAnimationPlayer = GetNode<AnimationPlayer>("Rocket/AnimationPlayer");
            AltitudeProgressBar = GetNode<ProgressBar>("HUD/Altitude/ProgressBar");
            AltitudeLabel = GetNode<Label>("HUD/Altitude/Label");

            AltitudeProgressBar.MaxValue = 1000;
        }

        public override void _Process(float delta)
        {
            if (Logic.Altitude <= 0)
                EndGame();

            if (isHolding)
                pendingFuel += delta * FUEL_PENDING_SPEED;

            UpdateHud();
        }

        public override void _Input(InputEvent inputEvent)
        {
            if (inputEvent.IsActionPressed("restart"))
                GetTree().ReloadCurrentScene();

            if (inputEvent is InputEventScreenTouch touchEvent)
            {
                if (touchEvent.Pressed)
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
                        .TweenProperty(Rocket, "rect_position", new Vector2(Rocket.RectPosition.x, GameLogic.INITIAL_ALTITUDE - Logic.Altitude), 0.5f)
                        .SetTrans(Tween.TransitionType.Quart)
                        .SetEase(Tween.EaseType.Out);
                    pendingFuel = 0;
                }
            }
        }

        private void UpdateHud()
        {
            AltitudeProgressBar.Value = Logic.Altitude;
            AltitudeLabel.Text = $"{(int)Logic.Altitude}m";

            // TODO: should be removed or hidden
            GetNode<Label>("Info").Text = $"Velocity:{Logic.Velocity}\nFuelRemaining:{Logic.FuelRemaining}\npendingFuel:{pendingFuel}";
        }

        private void EndGame()
        {
        }
    }
}