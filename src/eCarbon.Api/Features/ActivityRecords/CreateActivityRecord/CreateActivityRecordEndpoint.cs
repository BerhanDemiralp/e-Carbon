using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eCarbon.Api.Features.ActivityRecords.CreateActivityRecord;

public static class CreateActivityRecordEndpoint
{
    public static IEndpointRouteBuilder MapCreateActivityRecord(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/activity-records", async (
            [FromBody] CreateActivityRecordRequest request,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = request.ToCommand();
            var result = await mediator.Send(command, ct);
            return Results.Created($"/api/activity-records/{result.Id}", result);
        })
        .WithName("CreateActivityRecord")
        .WithTags("Activity Records");

        return app;
    }
}
