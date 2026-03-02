using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payments.Entities.Wallet;

namespace Payments.Infrastructure.Persistence.Configurations;

public sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> b)
    {
        b.ToTable("reservations");
        b.HasKey(x => x.Id);

        b.Property(x => x.ReservationId).HasColumnName("reservation_id").IsRequired();
        b.HasIndex(x => x.ReservationId).IsUnique();

        b.Property(x => x.DealId).HasColumnName("deal_id").IsRequired();
        b.HasIndex(x => x.DealId).IsUnique();

        b.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        b.HasIndex(x => x.UserId);

        b.Property(x => x.Amount).HasColumnName("amount").HasColumnType("numeric(18,2)").IsRequired();
        b.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();

        b.Property(x => x.Status).HasColumnName("status").IsRequired();
        b.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
    }
}
