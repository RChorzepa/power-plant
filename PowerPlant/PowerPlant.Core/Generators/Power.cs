using System;
using System.Collections.Generic;
using System.Linq;
using PowerPlant.Core.Extensions;

namespace PowerPlant.Core.Generators
{
    public enum Unit
    {
        MW,
        kWh
    }

    public  class Power
    {
        private readonly Random _random = new Random();

        /// <summary>
        /// Generator max power in MW
        /// </summary>
        public int MaxPower { get;  private set; }

        public Power(int maxPower)
        {
            MaxPower = maxPower;
        }

        /// <summary>
        /// Generate random value in kWh unit
        /// </summary>
        /// <param name="unit">Power unit</param>
        /// <param name="interval">Time interval in miliseconds</param>
        /// <returns></returns>
       public float Generate(int interval)
        {
            var kwh = MaxPower * 1000;
            float maxValuePerMilisecond = kwh / 3600000f;

            var randomValue = _random.NextDouble(0, maxValuePerMilisecond);

            return (float)(randomValue * interval);
        }
    }
}
