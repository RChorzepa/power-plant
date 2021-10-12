using System.Threading.Tasks;

namespace PowerPlant.Core.Contracts
{
    public interface IAutoFillDataService
    {
        Task GenerateDataForYear(int year, int interval, int generators);
    }
}
