using Godot;
using System;

namespace LunarLander
{
    public class Game : Node
    {
        private const float FUEL_POWER = 0.5f;

        private bool isGameOver;

        private bool isUserHoldingDown;

        private float velocity = 0;

        private float altitude = 1000;

        private float fuelRemaining = 1000;

        private float pendingFuel = 0;

        public override void _Ready()
        {
        }

        public override void _PhysicsProcess(float delta)
        {
            if (isGameOver)
                return;

            velocity -= 0.02f;
            altitude += velocity;

            if (isUserHoldingDown && fuelRemaining > 0)
            {
                pendingFuel += 10;
                fuelRemaining -= 10;
            }
            else if (pendingFuel > 0)
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