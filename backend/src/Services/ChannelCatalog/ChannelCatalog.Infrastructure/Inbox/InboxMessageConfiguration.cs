using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChannelCatalog.Infrastructure.Inbox;

public sealed class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> b)
    {
        b.ToTable("inbox_messages");
        b.HasKey(x => x.Id);

        b.Property(x => x.MessageId).HasColumnName("message_id").HasMaxLength(64).IsRequired();
        b.Property(x => x.RoutingKey).HasColumnName("routing_key").HasMaxLength(200).IsRequired();
        b.Property(x => x.ProcessedAt).HasColumnName("processed_at").IsRequired();

        b.HasIndex(x => x.MessageId).IsUnique();
    }
}
