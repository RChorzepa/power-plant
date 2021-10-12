using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PowerPlant.Core.Contracts;
using PowerPlant.Core.Generators;
using PowerPlant.Core.Repositories;
using PowerPlant.Infrastructure.Persistence;
using PowerPlant.Infrastructure.Persistence.Repositories;
using PowerPlant.Infrastructure.Services.ProductionLogerService;

namespace PowerPlant.Infrastructure
{
    public static class Setup
    {
        public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PowerPlantDbContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("PowerPlantConnectionString")));

            services.AddScoped(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));
            services.AddScoped<IProductionLoggerService, FileProductionLoggerService>();
            services.Configure<GeneratorBusConfiguration>(opt => configuration.GetSection("Settings:Generator").Bind(opt));

            return services;
        }
    }
}
