using MediatR;

namespace eCarbon.Api.Features.ActivityRecords.ListAllActivityRecords;

public static class ListAllActivityRecordsEndpoint
{
    public static IEndpointRouteBuilder MapListAllActivityRecords(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/activity-records", async (
            IMediator mediator,
            CancellationToken ct) =>
        {
            var query = new ListAllActivityRecordsQuery();
            var result = await mediator.Send(query, ct);
            return Results.Ok(result);
        })
        .WithName("ListAllActivityRecords")
        .WithTags("Activity Records");

        return app;
    }
}