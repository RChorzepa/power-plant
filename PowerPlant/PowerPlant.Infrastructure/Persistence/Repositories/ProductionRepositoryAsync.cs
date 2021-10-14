using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PowerPlant.Core.Entities;
using PowerPlant.Core.Generators;
using PowerPlant.Core.Repositories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PowerPlant.Infrastructure.Persistence.Repositories
{
    public class ProductionRepositoryAsync : RepositoryAsync<Production>, IProductionRepositoryAsync
    {
        private readonly IConfiguration _configuration;

        public ProductionRepositoryAsync(PowerPlantDbContext dbContext, IConfiguration configuration) : base(dbContext)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Generate fake date for full year
        /// </summary>
        /// <param name="year"></param>
        /// <param name="generators">Generators count</param>
        /// <returns></returns>
        public async Task GenerateFakeDataAsync(int year, int generators)
        {
            var logs = new ConcurrentQueue<Production>();
            var from = new DateTime(year, 1, 1, 0, 0, 0, 0);
            var to = new DateTime(year, 12, DateTime.DaysInMonth(year, 12), 23, 59, 59);
            var totalSeconds = (to - from).TotalSeconds;

            var power = new Power(1);

                for (int i = 0; i < totalSeconds; i++)
                {
                    for (int j = 0; j < generators; j++)
                    {
                        logs.Enqueue(new Production(from.AddSeconds(i), j, power.Generate(1000)));
                    }
                }

                var batch = new List<Production>();

                while (logs.Any())
                {
                    Production prod;

                   if (logs.TryDequeue(out prod))
                        batch.Add(prod);

                    if (batch.Count == 10000)
                    {
                        await SaveDataBySqlBulkCopy(batch);
                        batch.Clear();
                    }
                        
                }

                if (batch.Any())
                    await SaveDataBySqlBulkCopy(batch);
        }

        private async Task SaveDataBySqlBulkCopy(List<Production> collection)
        {
           try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Id");
                dataTable.Columns.Add("GeneratorId");
                dataTable.Columns.Add("Quantity");
                dataTable.Columns.Add("Date");
                dataTable.Columns.Add("Time");

                int rowIndex = 0;

                for (int i = 0; i < collection.Count; i++)
                {
                    dataTable.Rows.Add(dataTable.NewRow());
                }

                foreach (var item in collection)
                {
                    DataRow dataRow = dataTable.Rows[rowIndex++];
                    dataRow["Id"] = null;
                    dataRow["GeneratorId"] = item.GeneratorId;
                    dataRow["Quantity"] = item.Quantity;
                    dataRow["Date"] = item.Date;
                    dataRow["Time"] = item.Time;
                }

                using (var connection = new SqlConnection(_configuration.GetConnectionString("PowerPlantConnectionString")))
                {
                    connection.Open();

                    SqlBulkCopy sqlBulk = new SqlBulkCopy(connection);
                    sqlBulk.DestinationTableName = "Productions";
                    await sqlBulk.WriteToServerAsync(dataTable);

                    connection.Close();
                }
            } catch(Exception ex)
            {

            }
        }

        public async Task<Dictionary<int, IEnumerable<(DateTime Date, double Avg)>>> GetReportByDatyAsync(DateTime date)
        {
            var models = new Dictionary<int, IEnumerable<(DateTime date, double avg)>>();

                var data = await  DbContext.Productions.ToListAsync();

                var groups = data.GroupBy(_ => _.GeneratorId, (id, values) => new
                {
                    Id = id,
                    Values = values.GroupBy(x => x.Date, (date, values) => new
                    {
                        Date = date,
                        Values = values.GroupBy(h => h.Time.Hours, (hour, values) => new
                        {
                            Hour = hour,
                            Values = values
                        })
                    })
                })
                .OrderBy(_ => _.Id);

                foreach (var group in groups)
                {
                    var reportModels = new List<(DateTime date, double avg)>();

                    foreach (var item in group.Values)
                    {
                        if (!item.Values.Any()) continue;

                        foreach (var hour in item.Values)
                        {
                            reportModels.Add((new DateTime(item.Date.Year, item.Date.Month, item.Date.Day, hour.Hour, 0 , 0), hour.Values.Average(_ => _.Quantity)));
                        }
                    }

                    models.Add(group.Id, reportModels.ToList());
                    reportModels.Clear();
                }

            return models;
        }
    }
}
