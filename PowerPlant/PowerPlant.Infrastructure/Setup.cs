using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PowerPlant.Core.Contracts;
using PowerPlant.Core.Generators;
using PowerPlant.Core.Repositories;
using PowerPlant.Infrastructure.Persistence;
using PowerPlant.Infrastructure.Persistence.Repositories;
using PowerPlant.Infrastructure.Services.AutoFillDataServices;
using PowerPlant.Infrastructure.Services.MailService;
using PowerPlant.Infrastructure.Services.MailService.Models;
using PowerPlant.Infrastructure.Services.ProductionLogerService;
using PowerPlant.Infrastructure.Services.ProductionPagedServices;

namespace PowerPlant.Infrastructure
{
    public static class Setup
    {
        public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PowerPlantDbContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("PowerPlantConnectionString")));

            services.Configure<GeneratorBusConfiguration>(opt => configuration.GetSection("Settings:Generator").Bind(opt));

            services.AddScoped<IMailService, SmtpMailService>();
            services.Configure<SmtpConfiguration>(opt => configuration.GetSection("Smtp").Bind(opt));

            services.AddScoped(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));
            services.AddScoped<IAutoFillDataService, AutoFillDataService>();

            services.AddScoped<IProductionRepositoryAsync, ProductionRepositoryAsync>();

            services.AddScoped<IProductionPagedService, ProductionPagedService>();
            services.AddSingleton<IProductionLoggerService, EFProductionLoggerService>();
            services.AddSingleton<IGeneratorBus, GeneratorBus>();

            return services;
        }
    }
}
