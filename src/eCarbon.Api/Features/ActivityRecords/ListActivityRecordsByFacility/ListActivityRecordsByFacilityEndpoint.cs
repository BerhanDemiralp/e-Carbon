using MediatR;

namespace eCarbon.Api.Features.ActivityRecords.ListActivityRecordsByFacility;

public static class ListActivityRecordsByFacilityEndpoint
{
    public static IEndpointRouteBuilder MapListActivityRecordsByFacility(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/facilities/{facilityId:guid}/activity-records", async (
            Guid facilityId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var query = new ListActivityRecordsByFacilityQuery(facilityId);
            var result = await mediator.Send(query, ct);
            return Results.Ok(result);
        })
        .WithName("ListActivityRecordsByFacility")
        .WithTags("Activity Records");

        return app;
    }
}