using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PowerPlant.Core.Contracts;
using PowerPlant.Core.Generators;
using PowerPlant.Core.Repositories;
using PowerPlant.Infrastructure.Persistence;
using PowerPlant.Infrastructure.Persistence.Repositories;
using PowerPlant.Infrastructure.Services.ProductionLogerService;
using Microsoft.Extensions.Configuration;
using PowerPlant.Infrastructure.Services;

namespace PowerPlant.Tests
{
    public class Startup
    {
        public static IConfigurationRoot ConfigurationRoot()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, true)
                .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = ConfigurationRoot();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddDbContext<PowerPlantDbContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("PowerPlantConnectionString_TEST")));
            services.AddScoped(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));
            services.AddScoped<IProductionRepositoryAsync, ProductionRepositoryAsync>();
            services.AddScoped<IProductionLoggerService, EFProductionLoggerService>();
            services.Configure<GeneratorBusConfiguration>(opt => configuration.GetSection("Settings:Generator").Bind(opt));
            services.AddSingleton<GeneratorBus>();
            services.AddSingleton<IGeneratorBus, GeneratorBus>();
            services.AddScoped<IAutoFillDataService, AutoFillDataService>();
        }
    }
}
