using PowerPlant.Core.Contracts;
using PowerPlant.Core.Entities;
using PowerPlant.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerPlant.Infrastructure.Services.ProductionLogerService
{
    public class EFProductionLoggerService : IProductionLoggerService
    {
        private readonly IRepositoryAsync<Production> _productionRepository;
        private readonly IRepositoryAsync<Notification> _notificationRepository;

        public EFProductionLoggerService(IRepositoryAsync<Production> productionRepository, IRepositoryAsync<Notification> notificationRepository)
        {
            _productionRepository = productionRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task LogMessage(string message)
        {
            await _notificationRepository.AddAsync(new Notification
            {
                Date = DateTime.Now,
                Message = message
            });
        }

        public async Task LogProduction(ICollection<Production> productions)
        {
            await _productionRepository.AddRangeAsync(productions);
        }
    }
}
