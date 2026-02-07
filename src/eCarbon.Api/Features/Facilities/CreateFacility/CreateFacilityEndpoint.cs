using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eCarbon.Api.Features.Facilities.CreateFacility;

public static class CreateFacilityEndpoint
{
    public static IEndpointRouteBuilder MapCreateFacility(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/companies/{companyId:guid}/facilities", async (
            Guid companyId,
            [FromBody] CreateFacilityRequest request,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = request.ToCommand(companyId);
            var result = await mediator.Send(command, ct);
            return Results.Created($"/api/facilities/{result.Id}", result);
        })
        .WithName("CreateFacility")
        .WithTags("Facilities")
        .Accepts<CreateFacilityRequest>("application/json");

        return app;
    }
}