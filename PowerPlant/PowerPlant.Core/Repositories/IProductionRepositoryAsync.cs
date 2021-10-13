using PowerPlant.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerPlant.Core.Repositories
{
    public interface IProductionRepositoryAsync : IRepositoryAsync<Production>
    {
        Task GenerateFakeDataAsync(int year, int generators);
        Task<Dictionary<int, IEnumerable<(DateTime Date, double Avg)>>> GetReportByDatyAsync(DateTime date);
    }
}
