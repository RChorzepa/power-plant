using PowerPlant.Core.Entities;
using System;

namespace PowerPlant.Core.Generators
{
    public class GeneratorConfiguration
    {
        /// <summary>
        /// Generator max power in MW
        /// </summary>
        public int MaxPower { get; set; }

        private Power _power;

        public Power Power
        {
            get
            {
                if (_power == null)
                    _power = new Power(MaxPower);

                return _power;
            }
        }

        /// <summary>
        /// Logging interval in miliseconds
        /// </summary>
        public int LoggingInterval { get; set; }

        public Action<Production> OnPowerGenerated;
    }
}
