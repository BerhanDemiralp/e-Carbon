using MediatR;

namespace eCarbon.Api.Features.Reports.VerifyReport;

public static class VerifyReportEndpoint
{
    public static IEndpointRouteBuilder MapVerifyReport(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/reports/{reportId:guid}/verify", async (
            Guid reportId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var query = new VerifyReportQuery(reportId);
            var result = await mediator.Send(query, ct);
            return Results.Ok(result);
        })
        .WithName("VerifyReport")
        .WithTags("Reports");

        return app;
    }
}
