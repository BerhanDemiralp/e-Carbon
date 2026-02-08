using MediatR;

namespace eCarbon.Api.Features.Reports.GenerateReport;

public static class GenerateReportEndpoint
{
    public static IEndpointRouteBuilder MapGenerateReport(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/snapshots/{snapshotId:guid}/reports", async (
            Guid snapshotId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = new GenerateReportCommand(snapshotId);
            var result = await mediator.Send(command, ct);
            return Results.Created($"/api/reports/{result.Id}", result);
        })
        .WithName("GenerateReport")
        .WithTags("Reports");

        return app;
    }
}
