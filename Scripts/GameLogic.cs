using System;
using System.Collections.Generic;

namespace LunarLander
{
    public class GameLogic
    {
        private const float FUEL_POWER = 0.15f;

        public float Velocity { get; private set; } = 0;

        public float Altitude { get; private set; } = 1000;

        public int FuelRemaining { get; private set; } = 1000;

        public int[] History => history.ToArray();

        private readonly List<int> history = new List<int>();

        /// <Summary>Continues the game by one in-game second.</Summary>
        public void Burn(int fuelToBurn)
        {
            if (fuelToBurn > FuelRemaining)
                fuelToBurn = FuelRemaining;

            FuelRemaining -= fuelToBurn;
            Velocity += 1.6f - (fuelToBurn * FUEL_POWER);
            Altitude = Math.Max(0, Altitude - Velocity);

            history.Add(fuelToBurn);
        }
    }
}