using PowerPlant.Core.Entities;
using PowerPlant.Infrastructure.Services.ProductionPagedServices.Models;

namespace PowerPlant.Infrastructure.Services.ProductionPagedServices
{
    public interface IProductionPagedService
    {
        Criterias GetFilterCriteraValues();
        Pagination<Production> GetPaged(string filter, int page = 1, int pageSize = 10);
    }
}
