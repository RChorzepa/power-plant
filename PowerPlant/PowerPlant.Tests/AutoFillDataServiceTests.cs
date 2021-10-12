using Microsoft.EntityFrameworkCore;
using PowerPlant.Core.Contracts;
using PowerPlant.Infrastructure.Persistence;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace PowerPlant.Tests
{
    public class AutoFillDataServiceTests
    {
        private readonly IAutoFillDataService _service;
        private readonly PowerPlantDbContext _powerPlantDbContext;

        public AutoFillDataServiceTests(IAutoFillDataService service, PowerPlantDbContext powerPlantDbContext)
        {
            _service = service;
            _powerPlantDbContext = powerPlantDbContext;
        }

        [Fact]
        public async Task GenerateDataForYear_CheckBulkICopyPerformance()
        {
            TruncateTable();

            var watch = new Stopwatch();
            watch.Start();

            await _service.GenerateDataForYear(2019, 1000, 1);

            watch.Stop();
            var seconds = watch.ElapsedMilliseconds / 1000;
        }

        private void TruncateTable()
        {
           using(var connection = _powerPlantDbContext.Database.GetDbConnection())
            {
                connection.Open();

               using( var command = connection.CreateCommand())
                {
                    command.CommandText = "TRUNCATE TABLE dbo.Productions";
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
