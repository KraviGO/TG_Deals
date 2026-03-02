using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Publishers.Entities.Channels;

namespace Publishers.Infrastructure.Persistence.Configurations;

public sealed class ChannelConfiguration : IEntityTypeConfiguration<Channel>
{
    public void Configure(EntityTypeBuilder<Channel> b)
    {
        b.ToTable("channels");
        b.HasKey(x => x.Id);

        b.Property(x => x.ChannelId)
            .HasConversion(id => id.Value, value => new ChannelId(value))
            .HasColumnName("channel_id")
            .IsRequired();
        b.HasIndex(x => x.ChannelId).IsUnique();

        b.Property(x => x.PublisherUserId).HasColumnName("publisher_user_id").IsRequired();

        b.Property(x => x.TelegramChannelId).HasColumnName("telegram_channel_id").HasMaxLength(128).IsRequired();
        b.Property(x => x.Title).HasColumnName("title").HasMaxLength(200).IsRequired();

        b.Property(x => x.IntakeMode).HasColumnName("intake_mode").IsRequired();
        b.Property(x => x.OwnershipStatus).HasColumnName("ownership_status").IsRequired();

        b.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();

        b.Property(x => x.VerificationCode).HasColumnName("verification_code").HasMaxLength(64);
        b.Property(x => x.VerificationExpiresAt).HasColumnName("verification_expires_at");

        b.HasIndex(x => new { x.PublisherUserId, x.TelegramChannelId }).IsUnique();
    }
}
