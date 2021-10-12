using PowerPlant.Core.Contracts;
using PowerPlant.Core.Entities;
using PowerPlant.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace PowerPlant.Infrastructure.Services.ProductionLogerService
{
    public class EFProductionLoggerService : IProductionLoggerService
    {

        private readonly IServiceProvider _serviceProvider;

        public EFProductionLoggerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task LogMessage(string message)
        {
            using(var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<PowerPlantDbContext>();

                await dbContext.Notifications.AddAsync(new Notification
                {
                    Date = DateTime.Now,
                    Message = message
                });

                await dbContext.SaveChangesAsync();
            }

            await Task.CompletedTask;
        }

        public async Task LogProduction(ICollection<Production> productions)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<PowerPlantDbContext>();

                await dbContext.Productions.AddRangeAsync(productions);
                await dbContext.SaveChangesAsync();
            }

        }
    }
}
