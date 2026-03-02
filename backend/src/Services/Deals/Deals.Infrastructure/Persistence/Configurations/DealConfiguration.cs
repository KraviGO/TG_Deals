using Deals.Entities.Deals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deals.Infrastructure.Persistence.Configurations;

public sealed class DealConfiguration : IEntityTypeConfiguration<Deal>
{
    public void Configure(EntityTypeBuilder<Deal> b)
    {
        b.ToTable("deals");
        b.HasKey(x => x.Id);

        b.Property(x => x.DealId).HasColumnName("deal_id").IsRequired();
        b.HasIndex(x => x.DealId).IsUnique();

        b.Property(x => x.ChannelId).HasColumnName("channel_id").IsRequired();
        b.Property(x => x.PublisherUserId).HasColumnName("publisher_user_id").IsRequired();
        b.Property(x => x.AdvertiserUserId).HasColumnName("advertiser_user_id").IsRequired();
        b.Property(x => x.PaymentId).HasColumnName("payment_id");
        b.Property(x => x.PaymentState).HasColumnName("payment_state").HasMaxLength(50);

        b.Property(x => x.PostText).HasColumnName("post_text").HasColumnType("text").IsRequired();
        b.Property(x => x.DesiredPublishAtUtc).HasColumnName("desired_publish_at_utc").IsRequired();

        b.Property(x => x.Status).HasColumnName("status").IsRequired();

        b.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();

        b.HasIndex(x => x.AdvertiserUserId);
        b.HasIndex(x => x.PublisherUserId);
        b.HasIndex(x => x.ChannelId);
    }
}
