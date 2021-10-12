using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PowerPlant.Core.Entities;

namespace PowerPlant.Infrastructure.Persistence.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.Property(_ => _.Date).IsRequired();
            builder.Property(_ => _.Message).IsRequired().HasMaxLength(500);
        }
    }
}
