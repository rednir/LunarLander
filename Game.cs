using Godot;
using System;

namespace LunarLander
{
    public class Game : Node
    {
        private const float FUEL_POWER = 0.1f;

        private bool isGameOver;

        private bool isUserHoldingDown;

        private float velocity = 0;

        private float altitude = 1000;

        private float fuelRemaining = 1000;

        private float pendingFuel = 0;

        private ParallaxBackground StarsParallax;

        private Sprite Background;

        public override void _Ready()
        {
            StarsParallax = GetNode<ParallaxBackground>("StarsParallax");
            Background = GetNode<Sprite>("CanvasLayer/Background");
        }

        public override void _PhysicsProcess(float delta)
        {
            if (isGameOver)
                return;

            // TODO: Background offset
            StarsParallax.ScrollOffset = new Vector2(0, altitude);

            velocity -= 0.08f;
            altitude += velocity;

            if (isUserHoldingDown && fuelRemaining > 0 && pendingFuel < 100)
            {
                pendingFuel += 10;
                fuelRemaining -= 10;
            }

            if (!isUserHoldingDown && pendingFuel > 0)
            {
                velocity += pendingFuel * FUEL_POWER;
                pendingFuel = 0;
            }

            GetNode<Label>("Info").Text = $"Velocity:{velocity}\nAltitude:{altitude}\nFuelRemaining:{fuelRemaining}\nPendingFuel:{pendingFuel}\nIsUserHoldingDown:{isUserHoldingDown}";

            if (altitude <= 0)
                EndGame();
        }

        public override void _Input(InputEvent inputEvent) =>
            isUserHoldingDown = (inputEvent as InputEventMouseButton)?.Pressed ?? false;

        private void EndGame()
        {
            isGameOver = true;
            altitude = 0;
        }
    }
}