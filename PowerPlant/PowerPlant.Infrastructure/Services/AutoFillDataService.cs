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

namespace PowerPlant.Infrastructure.Services
{
    public class TimeRangeMemo
    {
        private Dictionary<(int, int), List<DateTime>> _memo = new Dictionary<(int, int), List<DateTime>>();

        /// <summary>
        /// Generate time range for year by 
        /// </summary>
        /// <param name="year"></param>
        /// <param name="interval">Time interval in miliseconds</param>
        /// <returns></returns>
        public List<DateTime> GetRange(int year, int interval)
        {
            if (!_memo.ContainsKey((year, interval)))
            {
                var range = GenerateRange(year, interval);
                _memo.Add((year, interval), range);
            }

            return _memo[(year, interval)];
        }

        private List<DateTime> GenerateRange(int year, int interval)
        {
            var template = new List<DateTime>();
            var current = new DateTime(year, 1, 1, 0, 0, 0, 0);
            var to = new DateTime(year, 12, DateTime.DaysInMonth(year, 1), 23, 59, 59);

            while (current != to)
            {
                template.Add(current);
                current = current.AddMilliseconds(interval);
            }

            return template;
        }
    }

    public class ProductionDataTableMemo
    {
        private Dictionary<int, DataTable> _memo = new Dictionary<int, DataTable>();

        public DataTable GetDataTableTemplate(int size)
        {
            if (!_memo.ContainsKey(size))
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Id");
                dataTable.Columns.Add("GeneratorId");
                dataTable.Columns.Add("Quantity");
                dataTable.Columns.Add("Date");
                dataTable.Columns.Add("Time");

                for (int i = 0; i < size; i++)
                    dataTable.Rows.Add(dataTable.NewRow());

                _memo.Add(size, dataTable);
            }

            return _memo[size];
        }
    }

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
