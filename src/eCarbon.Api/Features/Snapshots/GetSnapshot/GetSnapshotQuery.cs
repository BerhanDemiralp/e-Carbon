using MediatR;

namespace eCarbon.Api.Features.Snapshots.GetSnapshot;

public record GetSnapshotQuery(Guid SnapshotId) : IRequest<GetSnapshotResponse>;

public record GetSnapshotResponse(
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
    List<SnapshotItemDto> Items);

public record SnapshotItemDto(
    Guid Id,
    string FacilityName,
    DateTime ActivityDate,
    string ActivityType,
    int Scope,
    decimal Quantity,
    string Unit,
    decimal FactorKgPerUnit,
    decimal Co2eKg);