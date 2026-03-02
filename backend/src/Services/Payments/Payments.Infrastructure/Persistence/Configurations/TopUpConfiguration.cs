using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payments.Entities.TopUps;

namespace Payments.Infrastructure.Persistence.Configurations;

public sealed class TopUpConfiguration : IEntityTypeConfiguration<TopUp>
{
    public void Configure(EntityTypeBuilder<TopUp> b)
    {
        b.ToTable("topups");
        b.HasKey(x => x.Id);

        b.Property(x => x.TopUpId).HasColumnName("topup_id").IsRequired();
        b.HasIndex(x => x.TopUpId).IsUnique();

        b.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        b.HasIndex(x => x.UserId);

        b.Property(x => x.Amount).HasColumnName("amount").HasColumnType("numeric(18,2)").IsRequired();
        b.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();

        b.Property(x => x.YooKassaPaymentId).HasColumnName("yookassa_payment_id").HasMaxLength(64).IsRequired();
        b.HasIndex(x => x.YooKassaPaymentId).IsUnique();

        b.Property(x => x.ConfirmationUrl).HasColumnName("confirmation_url").HasColumnType("text");

        b.Property(x => x.Status).HasColumnName("status").IsRequired();

        b.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
    }
}
