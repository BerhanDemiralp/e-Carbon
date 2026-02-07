using MediatR;

namespace eCarbon.Api.Features.Facilities.GetFacility;

public static class GetFacilityEndpoint
{
    public static IEndpointRouteBuilder MapGetFacility(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/facilities/{id:guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var query = new GetFacilityQuery(id);
            var result = await mediator.Send(query, ct);
            return Results.Ok(result);
        })
        .WithName("GetFacility");

        return app;
    }
}