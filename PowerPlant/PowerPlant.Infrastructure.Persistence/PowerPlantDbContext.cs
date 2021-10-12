using Microsoft.EntityFrameworkCore;
using PowerPlant.Core.Entities;

namespace PowerPlant.Infrastructure.Persistence
{
    public class PowerPlantDbContext : DbContext
    {
        public PowerPlantDbContext(DbContextOptions<PowerPlantDbContext> options) : base(options)
        {

        }

        public DbSet<Production> Productions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Production>() .Property(_ => _.Quantity).IsRequired();
            modelBuilder.Entity<Production>().Property(_ => _.Date).IsRequired();
            modelBuilder.Entity<Production>().Property(_ => _.Time).IsRequired();
        }
    }
}
