using MediatR;

namespace eCarbon.Api.Features.Snapshots.CreateSnapshot;

public record CreateSnapshotCommand(Guid CompanyId, string Month) : IRequest<CreateSnapshotResponse>;

public record CreateSnapshotResponse(
    Guid Id,
    Guid CompanyId,
    string Month,
    string Status,
    decimal Scope1TotalKg,
    decimal Scope2TotalKg,
    decimal TotalKg,
    DateTime CreatedAt,
    int ItemCount);