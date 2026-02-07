using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eCarbon.Api.Features.Snapshots.GetSnapshot;

public static class GetSnapshotEndpoint
{
    public static IEndpointRouteBuilder MapGetSnapshot(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/snapshots/{snapshotId:guid}", async (
            Guid snapshotId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var query = new GetSnapshotQuery(snapshotId);
            var result = await mediator.Send(query, ct);
            return Results.Ok(result);
        })
        .WithName("GetSnapshot")
        .WithTags("Snapshots")
        .Produces<GetSnapshotResponse>(StatusCodes.Status200OK);

        return app;
    }
}
