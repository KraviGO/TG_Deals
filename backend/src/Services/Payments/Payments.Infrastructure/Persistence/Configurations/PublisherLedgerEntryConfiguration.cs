using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payments.Entities.PublisherLedger;

namespace Payments.Infrastructure.Persistence.Configurations;

public sealed class PublisherLedgerEntryConfiguration : IEntityTypeConfiguration<PublisherLedgerEntry>
{
    public void Configure(EntityTypeBuilder<PublisherLedgerEntry> b)
    {
        b.ToTable("publisher_ledger_entries");
        b.HasKey(x => x.Id);

        b.Property(x => x.EntryId).HasColumnName("entry_id").IsRequired();
        b.HasIndex(x => x.EntryId).IsUnique();

        b.Property(x => x.DealId).HasColumnName("deal_id").IsRequired();
        b.HasIndex(x => x.DealId).IsUnique();

        b.Property(x => x.PublisherUserId).HasColumnName("publisher_user_id").IsRequired();
        b.HasIndex(x => x.PublisherUserId);

        b.Property(x => x.GrossAmount).HasColumnName("gross_amount").HasColumnType("numeric(18,2)").IsRequired();
        b.Property(x => x.PlatformFeeAmount).HasColumnName("platform_fee_amount").HasColumnType("numeric(18,2)").IsRequired();
        b.Property(x => x.PublisherAmount).HasColumnName("publisher_amount").HasColumnType("numeric(18,2)").IsRequired();

        b.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        b.Property(x => x.Status).HasColumnName("status").IsRequired();

        b.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        b.Property(x => x.AvailableAt).HasColumnName("available_at");

        b.HasIndex(x => x.Status);
    }
}
