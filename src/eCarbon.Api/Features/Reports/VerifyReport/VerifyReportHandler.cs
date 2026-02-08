using eCarbon.Api.Common.Exceptions;
using eCarbon.Api.Common.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCarbon.Api.Features.Reports.VerifyReport;

public class VerifyReportHandler : IRequestHandler<VerifyReportQuery, VerifyReportResponse>
{
    private readonly AppDbContext _dbContext;

    public VerifyReportHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<VerifyReportResponse> Handle(VerifyReportQuery request, CancellationToken cancellationToken)
    {
        var report = await _dbContext.Reports
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
        
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashBytes = sha256.ComputeHash(fileContent);
        var computedHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

        var isValid = computedHash.Equals(report.HashValue, StringComparison.OrdinalIgnoreCase);

        return new VerifyReportResponse(
            report.Id,
            report.SnapshotId,
            report.HashAlgorithm,
            report.HashValue,
            computedHash,
            isValid,
            DateTime.UtcNow
        );
    }
}
