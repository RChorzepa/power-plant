using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PowerPlant.Core.Contracts;
using PowerPlant.Core.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PowerPlant.Core.Generators
{

    public class GeneratorBus : IGeneratorBus
    {
        private readonly IProductionLoggerService _productionLoggerService;

        private GeneratorBusConfiguration _configuration;
        private Action<Production> _onGeneratePower;
        private ConcurrentQueue<Production> _productions = new ConcurrentQueue<Production>();
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public List<Generator> Generators { get; private set; } = new List<Generator>();

        //public GeneratorBus(IServiceProvider serviceProvider, IOptions<GeneratorBusConfiguration> configuration)
        //{
        //    using (var scope = serviceProvider.CreateScope())
        //    {
        //        _productionLoggerService = scope.ServiceProvider.GetService<IProductionLoggerService>(); ;
        //    }

        //    _onGeneratePower = value => _productions.Enqueue(value);
        //    _configuration = configuration.Value;

        //    CreateGenerators();
        //}

        public GeneratorBus(IProductionLoggerService productionLoggerService, IOptions<GeneratorBusConfiguration> configuration)
        {
            _productionLoggerService = productionLoggerService;
            _onGeneratePower = value => _productions.Enqueue(value);
            _configuration = configuration.Value;

            CreateGenerators();
        }

        public void Start()
        {
            Task.Run(() =>
            {
                Generators.ForEach(_ =>
                {
                    _productionLoggerService.LogMessage($"Generator: {_.Id} | {Generators.Count - 1}-  START");
                    _.Start();
                });

                var batch = new List<Production>();

                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    if (_productions.Any())
                    {
                        if (_productions.TryDequeue(out Production production))
                        {
                            batch.Add(production);
                            if (batch.Count == _configuration.BatchSize)
                            {
                                _productionLoggerService.LogProduction(batch);
                                batch.Clear();
                            }
                        }
                    }
                }

                if (batch.Any())
                    _productionLoggerService.LogProduction(batch);

            });
        }

        public async Task Stop()
        {
            foreach (var item in Generators)
            {
                await _productionLoggerService.LogMessage($"Generator: {item.Id} | {Generators.Count - 1}-  STOP");
            }
            _cancellationTokenSource.Cancel();
        }

        private void CreateGenerators()
        {
            _configuration.GeneratorProperties.OnPowerGenerated = _onGeneratePower;

            for (int index = 0; index < _configuration.Quantity; index++)
            {
                Generators.Add(new Generator(index, _configuration.GeneratorProperties, _cancellationTokenSource.Token));
            }
        }

    }
}
