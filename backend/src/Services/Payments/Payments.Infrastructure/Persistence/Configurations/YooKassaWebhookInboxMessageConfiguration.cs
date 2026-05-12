using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payments.Entities.Webhooks;

namespace Payments.Infrastructure.Persistence.Configurations;

public sealed class YooKassaWebhookInboxMessageConfiguration : IEntityTypeConfiguration<YooKassaWebhookInboxMessage>
{
    public void Configure(EntityTypeBuilder<YooKassaWebhookInboxMessage> b)
    {
        b.ToTable("yookassa_webhook_inbox");
        b.HasKey(x => x.Id);

        b.Property(x => x.MessageId).HasColumnName("message_id").HasMaxLength(200).IsRequired();
        b.HasIndex(x => x.MessageId).IsUnique();

        b.Property(x => x.EventType).HasColumnName("event_type").HasMaxLength(128).IsRequired();
        b.Property(x => x.YooKassaPaymentId).HasColumnName("yookassa_payment_id").HasMaxLength(64).IsRequired();
        b.Property(x => x.RemoteIp).HasColumnName("remote_ip").HasMaxLength(64);
        b.Property(x => x.ProcessedAt).HasColumnName("processed_at").IsRequired();

        b.HasIndex(x => x.ProcessedAt);
    }
}
