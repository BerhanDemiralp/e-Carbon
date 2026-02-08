using eCarbon.Api.Common.Exceptions;
using eCarbon.Api.Common.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCarbon.Api.Features.Reports.DownloadReport;

public class DownloadReportHandler : IRequestHandler<DownloadReportQuery, DownloadReportResponse>
{
    private readonly AppDbContext _dbContext;

    public DownloadReportHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DownloadReportResponse> Handle(DownloadReportQuery request, CancellationToken cancellationToken)
    {
        var report = await _dbContext.Reports
            .Include(r => r.Snapshot)
                .ThenInclude(s => s.Company)
            .FirstOrDefaultAsync(r => r.Id == request.ReportId, cancellationToken);

        if (report == null)
        {
            throw new NotFoundException("Report", request.ReportId);
        }

        if (!File.Exists(report.PdfPath))
        {
            throw new NotFoundException("Report file", request.ReportId);
        }

        var fileContent = await File.ReadAllBytesAsync(report.PdfPath, cancellationToken);
        var sanitizedCompanyName = report.Snapshot.Company.Name.Replace(" ", "_");
        var month = report.Snapshot.Month.Replace("-", "_");
        var fileName = $"CarbonReport_{sanitizedCompanyName}_{month}.pdf";

        return new DownloadReportResponse(
            report.Id,
            report.SnapshotId,
            fileName,
            "application/pdf",
            fileContent
        );
    }
}
