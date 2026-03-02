using Marketplace.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payments.Infrastructure.Outbox;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> b)
    {
        b.ToTable("outbox_messages");
        b.HasKey(x => x.Id);

        b.Property(x => x.OccurredAt).HasColumnName("occurred_at").IsRequired();
        b.Property(x => x.EventType).HasColumnName("event_type").HasMaxLength(200).IsRequired();
        b.Property(x => x.Version).HasColumnName("version").IsRequired();

        b.Property(x => x.Exchange).HasColumnName("exchange").HasMaxLength(200).IsRequired();
        b.Property(x => x.RoutingKey).HasColumnName("routing_key").HasMaxLength(200).IsRequired();
        b.Property(x => x.PayloadJson).HasColumnName("payload_json").IsRequired();

        b.Property(x => x.Status).HasColumnName("status").IsRequired();
        b.Property(x => x.AttemptCount).HasColumnName("attempt_count").IsRequired();
        b.Property(x => x.LastError).HasColumnName("last_error");
        b.Property(x => x.PublishedAt).HasColumnName("published_at");
        b.Property(x => x.NextAttemptAt).HasColumnName("next_attempt_at");

        b.HasIndex(x => x.Status);
        b.HasIndex(x => x.OccurredAt);
        b.HasIndex(x => x.NextAttemptAt);
    }
}
