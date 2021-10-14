using LinqKit;
using System;
using PowerPlant.Core.Entities;
using PowerPlant.Infrastructure.Services.ProductionPagedServices.Models;
using System.Linq;

namespace PowerPlant.Infrastructure.Services.ProductionPagedServices
{
    public class ProductionPredicateBuilder
    {
        public ExpressionStarter<Production> Predicate = PredicateBuilder.New<Production>();

        #region Generators

        private int[] _generator;
        public int[] Generator
        {
            get => _generator;
            set
            {
                if(value.Any())
                {
                    _generator = value;
                    Predicate.And(_ => value.Contains(_.GeneratorId));
                }
            }
        }

        #endregion

        #region Date

        private Range<DateTime> _date;
        public Range<DateTime> Date
        {
            get => _date;
            set
            {
                if (value.Min != null && value.Max != null)
                {
                    _date = value;
                    Predicate.And(_ => _.Date >= value.Min && _.Date <= value.Max);
                }
            }
        }

        #endregion
    }
}
