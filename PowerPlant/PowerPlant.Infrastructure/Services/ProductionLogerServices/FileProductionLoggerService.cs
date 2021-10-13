using Newtonsoft.Json;
using PowerPlant.Core.Contracts;
using PowerPlant.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PowerPlant.Infrastructure.Services.ProductionLogerService
{
    public class FileProductionLoggerService : IProductionLoggerService
    {
        private string _path;

        public FileProductionLoggerService()
        {
            _path = GetFilePath();
        }

        private string GetFilePath()
        {
            var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "production_logs.txt");

            if (!File.Exists(path))
                File.Create(path);

            return path;
        }

        public async Task LogMessage(string message)
        {
            using StreamWriter file = new StreamWriter(_path, append: true);
            await file.WriteLineAsync($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} | {message}");
        }

        public async Task LogProduction(ICollection<Production> productions)
        {
            var log = new StringBuilder();

            int index = 1;
            log.AppendLine("----------------------------------------");
            foreach (var item in productions)
            {
                log.AppendLine($"{index} | Generator: {item.GeneratorId} | Date: {item.Date} | Time: {item.Time} | Quantity: {item.Quantity}");
                index++;
            }
            log.AppendLine("----------------------------------------");

            using StreamWriter file = new StreamWriter(_path, append: true);
            await file.WriteLineAsync(log.ToString());
        }
    }
}
