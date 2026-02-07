using MediatR;

namespace eCarbon.Api.Features.Facilities.ListAllFacilities;

public static class ListAllFacilitiesEndpoint
{
    public static IEndpointRouteBuilder MapListAllFacilities(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/facilities", async (
            IMediator mediator,
            CancellationToken ct) =>
        {
            var query = new ListAllFacilitiesQuery();
            var result = await mediator.Send(query, ct);
            return Results.Ok(result);
        })
        .WithName("ListAllFacilities")
        .WithTags("Facilities");

        return app;
    }
}