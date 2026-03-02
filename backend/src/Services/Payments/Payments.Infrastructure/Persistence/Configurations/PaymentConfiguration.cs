using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payments.Entities.Payments;

namespace Payments.Infrastructure.Persistence.Configurations;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> b)
    {
        b.ToTable("payments");
        b.HasKey(x => x.Id);

        b.Property(x => x.PaymentId).HasColumnName("payment_id").IsRequired();
        b.HasIndex(x => x.PaymentId).IsUnique();

        b.Property(x => x.DealId).HasColumnName("deal_id").IsRequired();
        b.HasIndex(x => x.DealId);

        b.Property(x => x.AdvertiserUserId).HasColumnName("advertiser_user_id").IsRequired();
        b.HasIndex(x => x.AdvertiserUserId);

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
