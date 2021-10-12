using System;

namespace PowerPlant.Core.Entities
{
    public class Production : Entity
    {
        public int GeneratorId { get; set; }
        public float Quantity { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }

        public Production(int generatorId, float quantity)
        {
            GeneratorId = generatorId;
            Quantity = quantity;
            Date = DateTime.Now;
            Time = DateTime.Now.TimeOfDay;
        }

        public Production(DateTime date, int generatorId, float quantity)
        {
            GeneratorId = generatorId;
            Quantity = quantity;
            Date = date;
            Time = date.TimeOfDay;
        }
    }
}
