using MediatR;

namespace eCarbon.Api.Features.Reports.GenerateReport;

public record GenerateReportCommand(Guid SnapshotId) : IRequest<GenerateReportResponse>;

public record GenerateReportResponse(
    Guid Id,
    Guid SnapshotId,
    string Status,
    DateTime CreatedAt,
    string HashAlgorithm,
    string HashValue);
