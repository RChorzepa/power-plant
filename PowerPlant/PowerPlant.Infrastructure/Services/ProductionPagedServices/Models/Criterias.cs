using System;

namespace PowerPlant.Infrastructure.Services.ProductionPagedServices.Models
{
    public class Criterias
    {
        public Range<DateTime> DateRange { get; set; }
        public int[] Generators { get; set; }
    }
}
