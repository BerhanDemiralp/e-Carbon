using MediatR;

namespace eCarbon.Api.Features.Facilities.ListFacilitiesByCompany;

public static class ListFacilitiesByCompanyEndpoint
{
    public static IEndpointRouteBuilder MapListFacilitiesByCompany(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/companies/{companyId:guid}/facilities", async (
            Guid companyId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var query = new ListFacilitiesByCompanyQuery(companyId);
            var result = await mediator.Send(query, ct);
            return Results.Ok(result);
        })
        .WithName("ListFacilitiesByCompany");

        return app;
    }
}