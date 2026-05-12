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

        b.Property(x => x.Amount).HasColumnName("amount").HasColumnType("numeric(18,2)").IsRequired();
        b.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        b.Property(x => x.FundingStatus).HasColumnName("funding_status").IsRequired();
        b.Property(x => x.ReservationId).HasColumnName("reservation_id");

        b.Property(x => x.PostUrl).HasColumnName("post_url").HasColumnType("text");
        b.Property(x => x.PublishedAtUtc).HasColumnName("published_at_utc");
        b.Property(x => x.PublisherComment).HasColumnName("publisher_comment").HasColumnType("text");



        b.Property(x => x.PostText).HasColumnName("post_text").HasColumnType("text").IsRequired();
        b.Property(x => x.DesiredPublishAtUtc).HasColumnName("desired_publish_at_utc").IsRequired();

        b.Property(x => x.Status).HasColumnName("status").IsRequired();

        b.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();

        b.HasIndex(x => x.AdvertiserUserId);
        b.HasIndex(x => x.PublisherUserId);
        b.HasIndex(x => x.ChannelId);
        b.HasIndex(x => x.ReservationId).IsUnique();
    }
}
