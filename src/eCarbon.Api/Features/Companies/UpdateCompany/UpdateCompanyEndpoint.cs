using MediatR;

namespace eCarbon.Api.Features.Companies.UpdateCompany;

public static class UpdateCompanyEndpoint
{
    public static IEndpointRouteBuilder MapUpdateCompany(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/companies/{id:guid}", async (
            Guid id,
            UpdateCompanyRequest request,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = request.ToCommand(id);
            var result = await mediator.Send(command, ct);
            return Results.Ok(result);
        })
        .WithName("UpdateCompany")
        .Produces<UpdateCompanyResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}