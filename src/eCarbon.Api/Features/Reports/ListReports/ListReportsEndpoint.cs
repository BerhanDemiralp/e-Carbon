using MediatR;

namespace eCarbon.Api.Features.Reports.ListReports;

public static class ListReportsEndpoint
{
    public static IEndpointRouteBuilder MapListReports(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/reports", async (
            IMediator mediator,
            CancellationToken ct) =>
        {
            var query = new ListReportsQuery();
            var result = await mediator.Send(query, ct);
            return Results.Ok(result);
        })
        .WithName("ListReports")
        .WithTags("Reports");

        return app;
    }
}
