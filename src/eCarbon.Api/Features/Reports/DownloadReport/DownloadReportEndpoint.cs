using MediatR;

namespace eCarbon.Api.Features.Reports.DownloadReport;

public static class DownloadReportEndpoint
{
    public static IEndpointRouteBuilder MapDownloadReport(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/reports/{reportId:guid}/download", async (
            Guid reportId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var query = new DownloadReportQuery(reportId);
            var result = await mediator.Send(query, ct);
            
            return Results.File(
                result.FileContent,
                contentType: result.ContentType,
                fileDownloadName: result.FileName
            );
        })
        .WithName("DownloadReport")
        .WithTags("Reports");

        return app;
    }
}
