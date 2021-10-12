using System.Threading;
using System.Threading.Tasks;
using PowerPlant.Core.Entities;

namespace PowerPlant.Core.Generators
{
    public class Generator
    {
        public readonly int Id;
        private readonly GeneratorConfiguration _configuration;
        private CancellationToken _cancellationToken;

        public Generator(int id, GeneratorConfiguration properties, CancellationToken cancellationToken)
        {
            Id = id;
            _configuration = properties;
            _cancellationToken = cancellationToken;
        }

        public void  Start()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    if (_cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    var production = new Production(Id, _configuration.Power.Generate(_configuration.LoggingInterval));
                    _configuration.OnPowerGenerated(production);
                    await Task.Delay(_configuration.LoggingInterval);      
                }                
            });
        }
    }
}
