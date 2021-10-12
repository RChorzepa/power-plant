using PowerPlant.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerPlant.Core.Contracts
{
    public interface IProductionLoggerService
    {
        Task LogMessage(string message);
        Task LogProduction(ICollection<Production> productions);
    }
}
