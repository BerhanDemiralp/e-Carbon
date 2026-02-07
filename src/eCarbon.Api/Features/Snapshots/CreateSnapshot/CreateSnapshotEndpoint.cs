using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eCarbon.Api.Features.Snapshots.CreateSnapshot;

public static class CreateSnapshotEndpoint
{
    public static IEndpointRouteBuilder MapCreateSnapshot(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/companies/{companyId:guid}/snapshots", async (
            Guid companyId,
            [FromBody] CreateSnapshotRequest request,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = request.ToCommand(companyId);
            var result = await mediator.Send(command, ct);
            return Results.Created($"/api/snapshots/{result.Id}", result);
        })
        .WithName("CreateSnapshot")
        .WithTags("Snapshots")
        .Accepts<CreateSnapshotRequest>("application/json");

        return app;
    }
}