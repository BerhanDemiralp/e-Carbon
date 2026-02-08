using MediatR;

namespace eCarbon.Api.Features.Reports.VerifyReport;

public record VerifyReportQuery(Guid ReportId) : IRequest<VerifyReportResponse>;

public record VerifyReportResponse(
    Guid Id,
    Guid SnapshotId,
    string HashAlgorithm,
    string StoredHash,
    string ComputedHash,
    bool IsValid,
    DateTime VerifiedAt);
