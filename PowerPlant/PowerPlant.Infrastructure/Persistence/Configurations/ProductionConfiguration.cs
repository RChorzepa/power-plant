using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PowerPlant.Core.Entities;

namespace PowerPlant.Infrastructure.Persistence.Configurations
{
    public class ProductionConfiguration : IEntityTypeConfiguration<Production>
    {
        public void Configure(EntityTypeBuilder<Production> builder)
        {
            builder.Property(_ => _.Quantity).IsRequired();
            builder.Property(_ => _.Date).IsRequired();
            builder.Property(_ => _.Time).IsRequired();
        }
    }
}
