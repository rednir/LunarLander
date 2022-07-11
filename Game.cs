using Godot;
using System;

namespace LunarLander
{
    public class Game : Node
    {
        private readonly GameLogic Logic = new GameLogic();

        private ParallaxBackground StarsParallax;

        private Sprite Background;

        private AnimationPlayer RocketAnimationPlayer;

        private ProgressBar AltitudeProgressBar;

        private Label AltitudeLabel;

        public override void _Ready()
        {
            StarsParallax = GetNode<ParallaxBackground>("StarsParallax");
            Background = GetNode<Sprite>("BackgroundLayer/Background");
            RocketAnimationPlayer = GetNode<AnimationPlayer>("Rocket/AnimationPlayer");
            AltitudeProgressBar = GetNode<ProgressBar>("HUD/Altitude/ProgressBar");
            AltitudeLabel = GetNode<Label>("HUD/Altitude/Label");

            AltitudeProgressBar.MaxValue = 1000;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (Logic.Altitude <= 0)
                EndGame();

            UpdateHud();
        }

        public override void _Input(InputEvent inputEvent)
        {
            if (inputEvent.IsActionPressed("restart"))
                GetTree().ReloadCurrentScene();
        }

        private void UpdateHud()
        {
            // TODO: background offset
            StarsParallax.ScrollOffset = new Vector2(0, Logic.Altitude);
            AltitudeProgressBar.Value = Logic.Altitude;
            AltitudeLabel.Text = $"{(int)Logic.Altitude}m";

            // TODO: should be removed or hidden
            GetNode<Label>("Info").Text = $"Velocity:{Logic.Velocity}\nFuelRemaining:{Logic.FuelRemaining}";
        }

        private void EndGame()
        {
        }
    }
}