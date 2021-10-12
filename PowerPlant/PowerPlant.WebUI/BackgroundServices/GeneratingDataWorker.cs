using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PowerPlant.Core.Generators;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PowerPlant.WebUI.Workers
{
    public class GeneratingDataWorker : IHostedService
    {
        private IGeneratorBus _generatorBus;
        private readonly IServiceProvider _serviceProvider;

        public GeneratingDataWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using(var scope = _serviceProvider.CreateScope())
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
