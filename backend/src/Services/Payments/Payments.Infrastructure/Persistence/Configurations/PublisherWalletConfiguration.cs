using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payments.Entities.PublisherLedger;

namespace Payments.Infrastructure.Persistence.Configurations;

public sealed class PublisherWalletConfiguration : IEntityTypeConfiguration<PublisherWallet>
{
    public void Configure(EntityTypeBuilder<PublisherWallet> b)
    {
        b.ToTable("publisher_wallets");
        b.HasKey(x => x.Id);

        b.Property(x => x.WalletId).HasColumnName("wallet_id").IsRequired();
        b.HasIndex(x => x.WalletId).IsUnique();

        b.Property(x => x.PublisherUserId).HasColumnName("publisher_user_id").IsRequired();
        b.HasIndex(x => x.PublisherUserId).IsUnique();

        b.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        b.Property(x => x.Available).HasColumnName("available").HasColumnType("numeric(18,2)").IsRequired();
        b.Property(x => x.PaidOut).HasColumnName("paid_out").HasColumnType("numeric(18,2)").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
    }
}
