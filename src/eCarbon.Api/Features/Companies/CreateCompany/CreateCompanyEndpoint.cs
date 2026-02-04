using MediatR;

namespace eCarbon.Api.Features.Companies.CreateCompany;

public static class CreateCompanyEndpoint
{
    public static IEndpointRouteBuilder MapCreateCompany(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/companies", async (
            CreateCompanyRequest request,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = request.ToCommand();
            var result = await mediator.Send(command, ct);
            return Results.Created($"/api/companies/{result.Id}", result);
        })
        .WithName("CreateCompany");

        return app;
    }
}