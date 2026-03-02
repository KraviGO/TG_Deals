using ChannelCatalog.Entities.Channels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChannelCatalog.Infrastructure.Persistence.Configurations;

public sealed class CatalogChannelConfiguration : IEntityTypeConfiguration<CatalogChannel>
{
    public void Configure(EntityTypeBuilder<CatalogChannel> b)
    {
        b.ToTable("catalog_channels");
        b.HasKey(x => x.Id);

        b.Property(x => x.ChannelId).HasColumnName("channel_id").IsRequired();
        b.HasIndex(x => x.ChannelId).IsUnique();

        b.Property(x => x.PublisherUserId).HasColumnName("publisher_user_id").IsRequired();

        b.Property(x => x.TelegramChannelId).HasColumnName("telegram_channel_id").HasMaxLength(128).IsRequired();
        b.Property(x => x.Title).HasColumnName("title").HasMaxLength(200).IsRequired();

        b.Property(x => x.IntakeMode).HasColumnName("intake_mode").HasMaxLength(50).IsRequired();
        b.Property(x => x.OwnershipStatus).HasColumnName("ownership_status").HasMaxLength(50).IsRequired();

        b.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();

        b.HasIndex(x => x.OwnershipStatus);
        b.HasIndex(x => x.IntakeMode);
    }
}
