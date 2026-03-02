using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payments.Entities.Wallet;

namespace Payments.Infrastructure.Persistence.Configurations;

public sealed class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
{
    public void Configure(EntityTypeBuilder<WalletTransaction> b)
    {
        b.ToTable("wallet_transactions");
        b.HasKey(x => x.Id);

        b.Property(x => x.TxId).HasColumnName("tx_id").IsRequired();
        b.HasIndex(x => x.TxId).IsUnique();

        b.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        b.HasIndex(x => x.UserId);

        b.Property(x => x.Type).HasColumnName("type").IsRequired();

        b.Property(x => x.Amount).HasColumnName("amount").HasColumnType("numeric(18,2)").IsRequired();
        b.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();

        b.Property(x => x.DealId).HasColumnName("deal_id");
        b.Property(x => x.TopUpId).HasColumnName("topup_id");

        b.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();

        b.HasIndex(x => x.DealId);
        b.HasIndex(x => x.TopUpId);
    }
}
