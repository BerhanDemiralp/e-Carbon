using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eCarbon.Api.Features.ActivityRecords.UpdateActivityRecord;

public static class UpdateActivityRecordEndpoint
{
    public static IEndpointRouteBuilder MapUpdateActivityRecord(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/activity-records/{id:guid}", async (
            Guid id,
            [FromBody] UpdateActivityRecordRequest request,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = request.ToCommand(id);
            var result = await mediator.Send(command, ct);
            return Results.Ok(result);
        })
        .WithName("UpdateActivityRecord")
        .WithTags("Activity Records")
        .Accepts<UpdateActivityRecordRequest>("application/json");

        return app;
    }
}