using MediatR;

namespace eCarbon.Api.Features.Companies.GetCompany;

public static class GetCompanyEndpoint
{
    public static IEndpointRouteBuilder MapGetCompany(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/companies/{id:guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var query = new GetCompanyQuery(id);
            var result = await mediator.Send(query, ct);
            return Results.Ok(result);
        })
        .WithName("GetCompany")
        .Produces<GetCompanyResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}