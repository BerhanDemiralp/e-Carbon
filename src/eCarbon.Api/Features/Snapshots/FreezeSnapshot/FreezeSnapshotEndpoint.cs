using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eCarbon.Api.Features.Snapshots.FreezeSnapshot;

public static class FreezeSnapshotEndpoint
{
    public static IEndpointRouteBuilder MapFreezeSnapshot(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/snapshots/{snapshotId:guid}/freeze", async (
            Guid snapshotId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = new FreezeSnapshotCommand(snapshotId);
            var result = await mediator.Send(command, ct);
            return Results.Ok(result);
        })
        .WithName("FreezeSnapshot")
        .WithTags("Snapshots")
        .Produces<FreezeSnapshotResponse>(StatusCodes.Status200OK);

        return app;
    }
}
