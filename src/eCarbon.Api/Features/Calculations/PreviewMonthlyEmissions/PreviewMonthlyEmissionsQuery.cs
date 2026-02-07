using MediatR;

namespace eCarbon.Api.Features.Calculations.PreviewMonthlyEmissions;

public record PreviewMonthlyEmissionsQuery(Guid CompanyId, string Month) : IRequest<PreviewMonthlyEmissionsResponse>;

public record PreviewMonthlyEmissionsResponse(
    Guid CompanyId,
    string CompanyName,
    string Month,
    decimal Scope1TotalKg,
    decimal Scope2TotalKg,
    decimal TotalKg,
    List<EmissionsBreakdownItem> Breakdown);

public record EmissionsBreakdownItem(
    Guid ActivityRecordId,
    string FacilityName,
    DateTime ActivityDate,
    string ActivityType,
    int Scope,
    decimal Quantity,
    string Unit,
    decimal FactorKgPerUnit,
    decimal Co2eKg);