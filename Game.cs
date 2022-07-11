using Godot;
using System;

namespace LunarLander
{
    public class Game : Node
    {
        private const float FUEL_POWER = 0.15f;

        private float velocity = 0;

        private float altitude = 1000;

        private float fuelRemaining = 1000;

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
            if (altitude <= 0)
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
            StarsParallax.ScrollOffset = new Vector2(0, altitude);
            AltitudeProgressBar.Value = altitude;
            AltitudeLabel.Text = $"{(int)altitude}m";

            // TODO: should be removed or hidden
            GetNode<Label>("Info").Text = $"Velocity:{velocity}\nFuelRemaining:{fuelRemaining}";
        }

        private void EndGame()
        {
            altitude = 0;
        }
    }
}