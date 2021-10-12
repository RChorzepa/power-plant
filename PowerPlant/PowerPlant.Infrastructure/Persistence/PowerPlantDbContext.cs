using Microsoft.EntityFrameworkCore;
using PowerPlant.Core.Entities;
using System.Reflection;

namespace PowerPlant.Infrastructure.Persistence
{
    public class PowerPlantDbContext : DbContext
    {
        public PowerPlantDbContext(DbContextOptions<PowerPlantDbContext> options) : base(options)
        {

        }

        public DbSet<Production> Productions { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
