using MediatR;

namespace eCarbon.Api.Features.Snapshots.ListSnapshots;

public static class ListSnapshotsEndpoint
{
    public static IEndpointRouteBuilder MapListSnapshots(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/snapshots", async (
            IMediator mediator,
            CancellationToken ct) =>
        {
            var query = new ListSnapshotsQuery();
            var result = await mediator.Send(query, ct);
            return Results.Ok(result);
        })
        .WithName("ListSnapshots")
        .WithTags("Snapshots");

        return app;
    }
}
