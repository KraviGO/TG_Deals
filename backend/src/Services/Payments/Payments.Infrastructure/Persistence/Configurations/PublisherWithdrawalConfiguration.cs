using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payments.Entities.PublisherLedger;

namespace Payments.Infrastructure.Persistence.Configurations;

public sealed class PublisherWithdrawalConfiguration : IEntityTypeConfiguration<PublisherWithdrawal>
{
    public void Configure(EntityTypeBuilder<PublisherWithdrawal> b)
    {
        b.ToTable("publisher_withdrawals");
        b.HasKey(x => x.Id);

        b.Property(x => x.WithdrawalId).HasColumnName("withdrawal_id").IsRequired();
        b.HasIndex(x => x.WithdrawalId).IsUnique();

        b.Property(x => x.PublisherUserId).HasColumnName("publisher_user_id").IsRequired();
        b.HasIndex(x => x.PublisherUserId);

        b.Property(x => x.Amount).HasColumnName("amount").HasColumnType("numeric(18,2)").IsRequired();
        b.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        b.Property(x => x.DestinationCardMask).HasColumnName("destination_card_mask").HasMaxLength(32).IsRequired();
        b.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
    }
}
