using MediatR;

namespace eCarbon.Api.Features.Companies.ListCompanies;

public static class ListCompaniesEndpoint
{
    public static IEndpointRouteBuilder MapListCompanies(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/companies", async (
            IMediator mediator,
            CancellationToken ct) =>
        {
            var query = new ListCompaniesQuery();
            var result = await mediator.Send(query, ct);
            return Results.Ok(result);
        })
        .WithName("ListCompanies");

        return app;
    }
}