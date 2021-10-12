using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerPlant.Core.Generators
{
    public interface IGeneratorBus
    {
        void Start();
        Task Stop();
        List<Generator> Generators { get; }
    }
}
