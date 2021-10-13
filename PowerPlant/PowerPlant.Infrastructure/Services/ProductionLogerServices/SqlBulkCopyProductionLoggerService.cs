using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PowerPlant.Core.Contracts;
using PowerPlant.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PowerPlant.Infrastructure.Services.ProductionLogerService
{
    public class SqlBulkCopyProductionLoggerService : IProductionLoggerService
    {
        private readonly ILogger<EFProductionLoggerService> _logger;
        private readonly IConfiguration _configuration;

        public SqlBulkCopyProductionLoggerService(ILogger<EFProductionLoggerService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task LogMessage(string message)
        {
            await Task.Run(() => _logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} | {message}"));
        }

        public async Task LogProduction(ICollection<Production> productions)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("PowerPlantConnectionString")))
            {
                int rowIndex = 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Id");
                dataTable.Columns.Add("GeneratorId");
                dataTable.Columns.Add("Quantity");
                dataTable.Columns.Add("Date");
                dataTable.Columns.Add("Time");

                for (int i = 0; i < productions.Count; i++)
                {
                    dataTable.Rows.Add(dataTable.NewRow());
                }

                foreach (var item in productions)
                {
                    DataRow dataRow = dataTable.Rows[rowIndex++];
                    dataRow["Id"] = null;
                    dataRow["GeneratorId"] = item.GeneratorId;
                    dataRow["Quantity"] = item.Quantity;
                    dataRow["Date"] = item.Date;
                    dataRow["Time"] = item.Time;
                }

                connection.Open();

                SqlBulkCopy sqlBulk = new SqlBulkCopy(connection);
                sqlBulk.DestinationTableName = "Productions";
                await sqlBulk.WriteToServerAsync(dataTable);
            }
        }
    }
}
