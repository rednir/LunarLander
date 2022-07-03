using Godot;
using System;

namespace LunarLander
{
    public class Game : Node
    {
        private const float FUEL_POWER = 0.1f;

        private const float COOLDOWN_TIME = 1f;

        private bool isGameOver;

        private bool isUserHoldingDown;

        private float velocity = 0;

        private float altitude = 1000;

        private float fuelRemaining = 1000;

        private float pendingFuel = 0;

        private float timeSinceLastBoost;

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
            if (isGameOver)
                return;

            if (altitude <= 0)
                EndGame();

            // TODO: Background offset
            StarsParallax.ScrollOffset = new Vector2(0, altitude);
            AltitudeProgressBar.Value = altitude;
            AltitudeLabel.Text = $"{(int)altitude}m";

            velocity -= 0.08f;
            altitude += velocity;

            timeSinceLastBoost += delta;

            if (isUserHoldingDown && fuelRemaining > 0 && pendingFuel < 100)
            {
                pendingFuel += 10;
                fuelRemaining -= 10;
            }

            if (!isUserHoldingDown && pendingFuel > 0)
            {
                velocity += pendingFuel * FUEL_POWER;
                pendingFuel = 0;
                timeSinceLastBoost = 0;
                RocketAnimationPlayer.Play("boost");
            }

            GetNode<Label>("Info").Text = $"Velocity:{velocity}\nFuelRemaining:{fuelRemaining}\nPendingFuel:{pendingFuel}\nTimeSinceLastBoost:{timeSinceLastBoost}\nIsUserHoldingDown:{isUserHoldingDown}";
        }

        public override void _Input(InputEvent inputEvent)
        {
            if (!isGameOver && inputEvent is InputEventScreenTouch touchEvent && timeSinceLastBoost > COOLDOWN_TIME)
            {
                if (touchEvent.Pressed && fuelRemaining > 0)
                {
                    isUserHoldingDown = true;
                    RocketAnimationPlayer.Play("holding");
                }
                else
                {
                    isUserHoldingDown = false;
                }
            }
        }

        private void EndGame()
        {
            isGameOver = true;
            altitude = 0;
        }
    }
}