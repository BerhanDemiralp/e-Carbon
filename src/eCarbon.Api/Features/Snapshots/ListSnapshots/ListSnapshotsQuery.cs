using MediatR;

namespace eCarbon.Api.Features.Snapshots.ListSnapshots;

public record ListSnapshotsQuery : IRequest<List<ListSnapshotsResponse>>;

public record ListSnapshotsResponse(
    Guid Id,
    Guid CompanyId,
    string CompanyName,
    string Month,
    string Status,
    decimal Scope1TotalKg,
    decimal Scope2TotalKg,
    decimal TotalKg,
    DateTime CreatedAt,
    DateTime? FrozenAt,
    int ItemCount);
