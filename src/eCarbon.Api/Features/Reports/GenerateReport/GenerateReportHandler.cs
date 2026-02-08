using eCarbon.Api.Common.Exceptions;
using eCarbon.Api.Common.Persistence;
using eCarbon.Api.Domain.Entities;
using eCarbon.Api.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace eCarbon.Api.Features.Reports.GenerateReport;

public class GenerateReportHandler : IRequestHandler<GenerateReportCommand, GenerateReportResponse>
{
    private readonly AppDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;

    public GenerateReportHandler(AppDbContext dbContext, IWebHostEnvironment environment)
    {
        _dbContext = dbContext;
        _environment = environment;
    }

    public async Task<GenerateReportResponse> Handle(GenerateReportCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await _dbContext.MonthlySnapshots
            .Include(s => s.Company)
            .Include(s => s.SnapshotItems)
                .ThenInclude(item => item.Facility)
            .FirstOrDefaultAsync(s => s.Id == request.SnapshotId, cancellationToken);

        if (snapshot == null)
        {
            throw new NotFoundException("Snapshot", request.SnapshotId);
        }

        if (snapshot.Status != SnapshotStatus.Frozen)
        {
            throw new InvalidOperationException("Snapshot must be frozen before generating a report");
        }

        var existingReport = await _dbContext.Reports
            .FirstOrDefaultAsync(r => r.SnapshotId == request.SnapshotId, cancellationToken);

        if (existingReport != null)
        {
            throw new InvalidOperationException("Report already exists for this snapshot");
        }

        var reportsPath = Path.Combine(_environment.ContentRootPath, "GeneratedReports");
        if (!Directory.Exists(reportsPath))
        {
            Directory.CreateDirectory(reportsPath);
        }

        var sanitizedCompanyName = snapshot.Company.Name.Replace(" ", "_");
        var month = snapshot.Month.Replace("-", "_");
        var pdfFileName = $"CarbonReport_{sanitizedCompanyName}_{month}.pdf";
        var pdfPath = Path.Combine(reportsPath, pdfFileName);
        var pdfBytes = GeneratePdfBytes(snapshot);
        await File.WriteAllBytesAsync(pdfPath, pdfBytes, cancellationToken);

        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashBytes = sha256.ComputeHash(pdfBytes);
        var hashValue = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

        var report = new Report
        {
            Id = Guid.NewGuid(),
            SnapshotId = request.SnapshotId,
            PdfPath = pdfPath,
            HashAlgorithm = "SHA-256",
            HashValue = hashValue,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Reports.Add(report);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new GenerateReportResponse(
            report.Id,
            report.SnapshotId,
            "Generated",
            report.CreatedAt,
            report.HashAlgorithm,
            report.HashValue
        );
    }

    private static byte[] GeneratePdfBytes(MonthlySnapshot snapshot)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(header => ComposeHeader(header, snapshot));
                page.Content().Element(content => ComposeContent(content, snapshot));
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private static void ComposeHeader(IContainer container, MonthlySnapshot snapshot)
    {
        container.Column(column =>
        {
            column.Item().Text("Carbon Emissions Report").FontSize(20).Bold();
            column.Item().PaddingTop(5).Text($"Company: {snapshot.Company.Name}").FontSize(12);
            column.Item().Text($"Period: {snapshot.Month}").FontSize(10);
            column.Item().Text($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC").FontSize(8);
            column.Item().PaddingTop(5).LineHorizontal(1).LineColor("#333333");
        });
    }

    private static void ComposeContent(IContainer container, MonthlySnapshot snapshot)
    {
        container.PaddingVertical(10).Column(column =>
        {
            column.Item().Text("Emissions Summary").FontSize(14).Bold();

            column.Item().PaddingBottom(5).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Scope 1").FontSize(10).Bold();
                    c.Item().Text($"{snapshot.Scope1TotalKg:F2} kg CO2e").FontSize(12);
                });
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Scope 2").FontSize(10).Bold();
                    c.Item().Text($"{snapshot.Scope2TotalKg:F2} kg CO2e").FontSize(12);
                });
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Total").FontSize(10).Bold();
                    c.Item().Text($"{snapshot.TotalKg:F2} kg CO2e").FontSize(12).Bold();
                });
            });

            column.Item().PaddingTop(15).Text("Activity Details").FontSize(14).Bold();

            foreach (var item in snapshot.SnapshotItems)
            {
                column.Item().PaddingTop(5).Row(row =>
                {
                    row.RelativeItem().Text(item.ActivityType.ToString());
                    row.RelativeItem().Text(item.Quantity.ToString("F2") + " " + item.Unit);
                    row.RelativeItem().Text(item.Co2eKg.ToString("F2") + " kg CO2e");
                });
            }
        });
    }

    private static void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().LineHorizontal(1).LineColor("#CCCCCC");
            column.Item().PaddingTop(5).Row(row =>
            {
                row.RelativeItem().Text("This report was generated by e-Carbon AI").FontSize(8);
                row.RelativeItem().AlignRight().Text("SHA-256 verified").FontSize(8);
            });
        });
    }
}
