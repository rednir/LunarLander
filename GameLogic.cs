using System;

namespace LunarLander
{
    public class GameLogic
    {
        public const float INITIAL_ALTITUDE = 1000;

        private const float FUEL_POWER = 0.15f;

        public float Velocity { get; private set; } = 0;

        public float Altitude { get; private set; } = INITIAL_ALTITUDE;

        public int FuelRemaining { get; private set; } = 1000;

        /// <Summary>Continues the game by one in-game second.</Summary>
        public void Burn(int fuelToBurn)
        {
            if (fuelToBurn > FuelRemaining)
                fuelToBurn = FuelRemaining;

            FuelRemaining -= fuelToBurn;
            Velocity += 1.6f - (fuelToBurn * FUEL_POWER);
            Altitude = Math.Max(0, Altitude - Velocity);
        }
    }
}