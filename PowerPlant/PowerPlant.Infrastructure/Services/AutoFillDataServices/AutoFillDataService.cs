using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PowerPlant.Core.Contracts;
using PowerPlant.Core.Entities;
using PowerPlant.Core.Generators;
using PowerPlant.Infrastructure.Persistence;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PowerPlant.Infrastructure.Services.AutoFillDataServices
{
    public class AutoFillDataService : IAutoFillDataService
    {
        private readonly string _connectionString;
        private readonly TimeRangeMemo _timeRange = new TimeRangeMemo();
        private readonly ProductionDataTableMemo _productionDataTableMemo = new ProductionDataTableMemo();

        private Power _power = new Power(1);
        private List<Production> _template = new List<Production>();

        public AutoFillDataService(PowerPlantDbContext dbContext)
        {
            _connectionString = dbContext.Database.GetDbConnection().ConnectionString;

            if (string.IsNullOrEmpty(_connectionString))
                throw new Exception("The connection string has not been initialized");
        }

        public async Task GenerateDataForYear(int year, int interval, int generators)
        {
            var range = _timeRange.GetRange(year, interval);

            var group = range
             .GroupBy(_ => _.Month, (month, days) => new
             {
                 Month = month,
                 Days = days.GroupBy(d => d.Day, (day, collection) => new
                 {
                     Day = day,
                     Collection = collection
                 })
             });

            foreach (var month in group)
                foreach (var day in month.Days)
                    foreach (var date in day.Collection)
                        _template.Add(new Production(date, 0, 0));


            await Execute(generators, interval);
        }

        private async Task Execute(int generators, int interval)
        {
            for (int generator = 0; generator < generators; generator++)
            {
                var counter = 0;

                while (counter < _template.Count)
                {
                    var batch = _template.Skip(counter).Take(10000).ToList();

                    batch.ForEach(_ =>
                    {
                        _.GeneratorId = generator;
                        _.Quantity = _power.Generate(interval);
                    });

                    counter += await SendData(batch);
                    batch.Clear();
                }
            }
        }

        private async Task<int> SendData(List<Production> data)
        {
            var dataTable = _productionDataTableMemo.GetDataTableTemplate(data.Count);

            int rowIndex = 0;

            foreach (var item in data)
            {
                DataRow dataRow = dataTable.Rows[rowIndex++];
                dataRow["Id"] = null;
                dataRow["GeneratorId"] = item.GeneratorId;
                dataRow["Quantity"] = item.Quantity;
                dataRow["Date"] = item.Date;
                dataRow["Time"] = item.Time;
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var sqlBulk = new SqlBulkCopy(connection);

                sqlBulk.DestinationTableName = "Productions";
                await sqlBulk.WriteToServerAsync(dataTable);

                connection.Close();

                return data.Count;
            }
        }

    }
}
