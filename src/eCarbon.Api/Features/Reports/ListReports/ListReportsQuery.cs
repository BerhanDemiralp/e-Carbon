using MediatR;

namespace eCarbon.Api.Features.Reports.ListReports;

public record ListReportsQuery : IRequest<List<ListReportsResponse>>;

public record ListReportsResponse(
    Guid Id,
    Guid SnapshotId,
    string CompanyName,
    string Month,
    string Status,
    decimal TotalKg,
    DateTime CreatedAt,
    string HashValue);
