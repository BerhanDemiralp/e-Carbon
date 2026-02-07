using MediatR;

namespace eCarbon.Api.Features.Snapshots.FreezeSnapshot;

public record FreezeSnapshotCommand(Guid SnapshotId) : IRequest<FreezeSnapshotResponse>;

public record FreezeSnapshotResponse(
    Guid Id,
    Guid CompanyId,
    string Month,
    string Status,
    decimal Scope1TotalKg,
    decimal Scope2TotalKg,
    decimal TotalKg,
    DateTime CreatedAt,
    DateTime FrozenAt);
