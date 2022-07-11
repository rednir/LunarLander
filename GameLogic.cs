using System;

namespace LunarLander
{
    public class GameLogic
    {
        private const float FUEL_POWER = 0.1f;

        public float Velocity { get; private set; } = 0;

        public float Altitude { get; private set; } = 1000;

        public float FuelRemaining { get; private set; } = 1000;

        /// <Summary>Continues the game by one in-game second.</Summary>
        public void BurnFuel(float fuelToBurn)
        {
            if (fuelToBurn > FuelRemaining)
                fuelToBurn = FuelRemaining;

            FuelRemaining -= fuelToBurn;
            Velocity += 1.6f - (fuelToBurn * FUEL_POWER);
            Altitude = Math.Max(0, Altitude - Velocity);
        }
    }
}