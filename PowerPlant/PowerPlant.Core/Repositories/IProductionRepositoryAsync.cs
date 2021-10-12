using PowerPlant.Core.Entities;
using System.Threading.Tasks;

namespace PowerPlant.Core.Repositories
{
    public interface IProductionRepositoryAsync : IRepositoryAsync<Production>
    {
        Task GenerateFakeData(int year, int generators);
    }
}
