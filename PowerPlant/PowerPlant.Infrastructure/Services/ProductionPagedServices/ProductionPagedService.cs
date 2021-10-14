using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;
using PowerPlant.Core.Entities;
using PowerPlant.Infrastructure.Persistence;
using PowerPlant.Infrastructure.Services.ProductionPagedServices.Models;

namespace PowerPlant.Infrastructure.Services.ProductionPagedServices
{
    public class ProductionPagedService : IProductionPagedService
    {
        private readonly PowerPlantDbContext _dbContext;

        public ProductionPagedService(PowerPlantDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Criterias GetFilterCriteraValues()
        {
            var generators = _dbContext.Productions.Select(_ => _.GeneratorId).Distinct().OrderBy(_ => _).ToArray();
            var dates = _dbContext.DatesRange.FirstOrDefault();

            return new Criterias
            {
                Generators = generators,
                DateRange = new Range<DateTime>
                {
                    Min = dates.Min.AddDays(-1),
                    Max = dates.Max
                }
            };
        }

        public Pagination<Production> GetPaged(string filter, int page = 1, int pageSize = 10)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var predicateBuilder = JsonConvert.DeserializeObject<ProductionPredicateBuilder>(filter, settings);
            var predicate = predicateBuilder.Predicate;

            var query = _dbContext.Productions.AsQueryable();
            var _page = (page < 0) ? 1 : page;
            var startRow = (_page - 1) * pageSize;

            int _count = predicate.IsStarted ? _dbContext.Productions.Count(predicate) : _dbContext.Productions.Count();

            var data = predicate.IsStarted
                ? query.Where(predicate).Skip(startRow).Take(pageSize).ToList()
                : query.Skip(startRow).Take(pageSize).ToList();

            return new Pagination<Production>
            {
                CurrentPage = _page,
                PageSize = pageSize,
                TotalItems = _count,
                TotalPages = (int)Math.Ceiling(_count / (double)pageSize),
                Items = data
            };

        }
    }
}
