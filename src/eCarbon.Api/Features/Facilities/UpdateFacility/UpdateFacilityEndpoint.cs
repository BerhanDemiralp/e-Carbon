using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eCarbon.Api.Features.Facilities.UpdateFacility;

public static class UpdateFacilityEndpoint
{
    public static IEndpointRouteBuilder MapUpdateFacility(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/facilities/{id:guid}", async (
            Guid id,
            [FromBody] UpdateFacilityRequest request,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = request.ToCommand(id);
            var result = await mediator.Send(command, ct);
            return Results.Ok(result);
        })
        .WithName("UpdateFacility")
        .Accepts<UpdateFacilityRequest>("application/json");

        return app;
    }
}