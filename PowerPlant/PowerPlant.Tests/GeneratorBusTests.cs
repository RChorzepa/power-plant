using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Options;
using PowerPlant.Core.Entities;
using PowerPlant.Core.Generators;
using PowerPlant.Core.Repositories;
using PowerPlant.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PowerPlant.Tests
{
    public class GeneratorBusTests
    {
        private readonly GeneratorBus _sut;
        private readonly PowerPlantDbContext _dbContext;
        private readonly IOptions<GeneratorBusConfiguration> _generatorBusConfiguration;

        public GeneratorBusTests(GeneratorBus generatorBus,
            PowerPlantDbContext dbContext,
            IOptions<GeneratorBusConfiguration> generatorBusConfiguration )
        {
            _sut = generatorBus;
            _dbContext = dbContext;
            _generatorBusConfiguration = generatorBusConfiguration;
        }

      [Fact]
      public void CreateGeneratorsFromConfigurationFile()
        {
            _sut.Generators.Count.Should().Be(10);
            _sut.Generators.Select(_ => _.Id)
                .ToArray()
                .Should()
                .OnlyHaveUniqueItems();
        }

        [Theory]
        [InlineData(10)]
        public async Task LogProductionDataIntoFile(int seconds)
        {
            _sut.Start();
            Thread.Sleep(seconds * 1000);
            await _sut.Stop();

            var production = _dbContext.Productions.ToList();

            var logs = production.GroupBy(_ => _.GeneratorId, (key, values) => new { GeneratorId = key, Values = values.ToList() });

            var interval = _generatorBusConfiguration.Value.GeneratorProperties.LoggingInterval;
            var maxLogsPerGenerator = (1000 / (float)interval) * seconds;

            using(new AssertionScope())
            {
                logs.Select(_ => _.GeneratorId).ToArray()
                    .Should()
                    .OnlyHaveUniqueItems()
                    .And
                    .HaveCount(_generatorBusConfiguration.Value.Quantity);

                foreach (var log in logs)
                {
                    log.Values.Count(_ => _.Quantity > 0).Should().BeLessOrEqualTo((int)maxLogsPerGenerator);
                }
            }

        }


        [Theory]
        [InlineData(1, 500)]
        [InlineData(1, 1000)]
        public void Power_GenerateRandomPower_ShouldNotExceedMaxPowerPerHour(int mw, int miliseconds)
        {
            var power = new Power(mw);
            var values = new List<float>();

            var start = new DateTime(2020, 1, 1, 1, 0, 0);
            var end = start.AddHours(1);

            while (!(start == end))
            {
                values.Add(power.Generate(miliseconds));
                start = start.AddMilliseconds(miliseconds);
            }

            var sum = values.Sum();

            sum.Should()
                .BeGreaterThan(0)
                .And
                .BeLessOrEqualTo(mw * 1000);
        }

    }
}
