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
        public DbSet<DatesRange> DatesRange { get; set; }
        public DbSet<RowsCount> RowsCount { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<DatesRange>(_ => _.HasNoKey().ToView("DatesRange"));
            modelBuilder.Entity<RowsCount>(_ => _.HasNoKey().ToView("RowsCount"));
        }
    }
}
