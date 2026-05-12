using Deals.Entities.Common;

namespace Deals.Entities.Disputes;

/// <summary>
/// Спор по сделке, который решает администратор.
/// </summary>
public sealed class DealDispute : Entity
{
    private DealDispute() { }

    public Guid DisputeId { get; private set; } = Guid.NewGuid();
    public Guid DealId { get; private set; }

    public Guid OpenedByUserId { get; private set; }
    public string OpenedByRole { get; private set; } = default!;
    public string Reason { get; private set; } = default!;

    public DisputeStatus Status { get; private set; }

    public Guid? ResolvedByUserId { get; private set; }
    public DisputeResolutionAction? ResolutionAction { get; private set; }
    public string? ResolutionNote { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? ResolvedAt { get; private set; }

    /// <summary>
    /// Открывает спор от рекламодателя или паблишера.
    /// </summary>
    public static DealDispute Open(
        Guid dealId,
        Guid openedByUserId,
        string openedByRole,
        string reason,
        DateTimeOffset nowUtc)
    {
        if (dealId == Guid.Empty) throw new ArgumentException("DealId required.");
        if (openedByUserId == Guid.Empty) throw new ArgumentException("OpenedByUserId required.");
        if (string.IsNullOrWhiteSpace(openedByRole)) throw new ArgumentException("OpenedByRole required.");
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Reason required.");

        return new DealDispute
        {
            Id = Guid.NewGuid(),
            DisputeId = Guid.NewGuid(),
            DealId = dealId,
            OpenedByUserId = openedByUserId,
            OpenedByRole = openedByRole.Trim(),
            Reason = reason.Trim(),
            Status = DisputeStatus.Open,
            CreatedAt = nowUtc,
            UpdatedAt = nowUtc
        };
    }

    /// <summary>
    /// Закрывает спор решением администратора.
    /// </summary>
    public void Resolve(Guid resolvedByUserId, DisputeResolutionAction action, string? note, DateTimeOffset nowUtc)
    {
        if (Status != DisputeStatus.Open)
            throw new InvalidOperationException("Dispute is not open.");

        ResolvedByUserId = resolvedByUserId;
        ResolutionAction = action;
        ResolutionNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        ResolvedAt = nowUtc;
        Status = DisputeStatus.Resolved;
        UpdatedAt = nowUtc;
    }
}
