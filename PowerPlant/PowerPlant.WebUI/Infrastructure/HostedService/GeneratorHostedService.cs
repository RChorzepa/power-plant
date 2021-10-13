using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PowerPlant.Core.Generators;
using PowerPlant.Core.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PowerPlant.WebUI.Infrastructure.HostedService
{
    public class GeneratorHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private IGeneratorBus _generatorBus;

        public GeneratorHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                _generatorBus = scope.ServiceProvider.GetService<IGeneratorBus>();
                _generatorBus.Start();
            }

            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _generatorBus.Stop();
        }

    }
}
