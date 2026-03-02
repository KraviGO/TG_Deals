using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payments.Entities.Wallet;

namespace Payments.Infrastructure.Persistence.Configurations;

public sealed class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> b)
    {
        b.ToTable("wallets");
        b.HasKey(x => x.Id);

        b.Property(x => x.WalletId).HasColumnName("wallet_id").IsRequired();
        b.HasIndex(x => x.WalletId).IsUnique();

        b.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        b.HasIndex(x => x.UserId).IsUnique();

        b.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        b.Property(x => x.Available).HasColumnName("available").HasColumnType("numeric(18,2)").IsRequired();
        b.Property(x => x.Reserved).HasColumnName("reserved").HasColumnType("numeric(18,2)").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
    }
}
