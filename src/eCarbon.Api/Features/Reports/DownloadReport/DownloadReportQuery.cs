using MediatR;

namespace eCarbon.Api.Features.Reports.DownloadReport;

public record DownloadReportQuery(Guid ReportId) : IRequest<DownloadReportResponse>;

public record DownloadReportResponse(
    Guid Id,
    Guid SnapshotId,
    string FileName,
    string ContentType,
    byte[] FileContent);
