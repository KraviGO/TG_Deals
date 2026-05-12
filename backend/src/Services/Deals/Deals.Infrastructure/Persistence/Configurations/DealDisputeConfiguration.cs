using Deals.Entities.Disputes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deals.Infrastructure.Persistence.Configurations;

public sealed class DealDisputeConfiguration : IEntityTypeConfiguration<DealDispute>
{
    public void Configure(EntityTypeBuilder<DealDispute> b)
    {
        b.ToTable("deal_disputes");
        b.HasKey(x => x.Id);

        b.Property(x => x.DisputeId).HasColumnName("dispute_id").IsRequired();
        b.HasIndex(x => x.DisputeId).IsUnique();

        b.Property(x => x.DealId).HasColumnName("deal_id").IsRequired();
        b.HasIndex(x => x.DealId).IsUnique();

        b.Property(x => x.OpenedByUserId).HasColumnName("opened_by_user_id").IsRequired();
        b.Property(x => x.OpenedByRole).HasColumnName("opened_by_role").HasMaxLength(32).IsRequired();
        b.Property(x => x.Reason).HasColumnName("reason").HasColumnType("text").IsRequired();

        b.Property(x => x.Status).HasColumnName("status").IsRequired();

        b.Property(x => x.ResolvedByUserId).HasColumnName("resolved_by_user_id");
        b.Property(x => x.ResolutionAction).HasColumnName("resolution_action");
        b.Property(x => x.ResolutionNote).HasColumnName("resolution_note").HasColumnType("text");

        b.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        b.Property(x => x.ResolvedAt).HasColumnName("resolved_at");
    }
}
