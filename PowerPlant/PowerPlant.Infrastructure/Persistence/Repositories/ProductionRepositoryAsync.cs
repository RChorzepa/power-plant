using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PowerPlant.Core.Entities;
using PowerPlant.Core.Generators;
using PowerPlant.Core.Repositories;
using System;
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
        public async Task GenerateFakeData(int year, int generators)
        {
            var logs = new Queue<Production>();
            var from = new DateTime(year, 1, 1, 0, 0, 0, 0);
            var to = new DateTime(year, 12, DateTime.DaysInMonth(year, 12), 23, 59, 59);
            var totalSeconds = (to - from).TotalSeconds;

            var power = new Power(1);

            for (int i = 0; i < totalSeconds; i++)
            {
                for (int j = 0; j < generators; j ++)
                {
                    logs.Enqueue(new Production(j, power.Generate(1000)));
                }
            }

            var batch = new List<Production>();

            while (logs.Any())
            {
                batch.Add(logs.Dequeue());

                if(batch.Count == 100000)
                    await SaveDataBySqlBulkCopy(batch);
            }

            if(batch.Any())
                await SaveDataBySqlBulkCopy(batch);

        }

        private async Task SaveDataBySqlBulkCopy(List<Production> collection)
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


            using(var connection = new SqlConnection(_configuration.GetConnectionString("Data Source=:memory")))
            {
                connection.Open();

                SqlBulkCopy sqlBulk = new SqlBulkCopy(connection);
                sqlBulk.DestinationTableName = "Productions";
                await sqlBulk.WriteToServerAsync(dataTable);
            }
        }
    }
}
