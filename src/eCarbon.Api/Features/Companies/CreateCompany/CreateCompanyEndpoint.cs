using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eCarbon.Api.Features.Companies.CreateCompany;

public static class CreateCompanyEndpoint
{
    public static IEndpointRouteBuilder MapCreateCompany(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/companies", async (
            [FromBody] CreateCompanyRequest request,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = request.ToCommand();
            var result = await mediator.Send(command, ct);
            return Results.Created($"/api/companies/{result.Id}", result);
        })
        .WithName("CreateCompany")
        .WithTags("Companies")
        .Accepts<CreateCompanyRequest>("application/json");

        return app;
    }
}