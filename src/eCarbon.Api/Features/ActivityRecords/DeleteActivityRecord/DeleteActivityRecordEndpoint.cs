using MediatR;

namespace eCarbon.Api.Features.ActivityRecords.DeleteActivityRecord;

public static class DeleteActivityRecordEndpoint
{
    public static IEndpointRouteBuilder MapDeleteActivityRecord(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/activity-records/{id:guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = new DeleteActivityRecordCommand(id);
            var result = await mediator.Send(command, ct);
            return Results.Ok(result);
        })
        .WithName("DeleteActivityRecord")
        .WithTags("Activity Records");

        return app;
    }
}