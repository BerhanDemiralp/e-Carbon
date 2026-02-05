using MediatR;

namespace eCarbon.Api.Features.Companies.DeleteCompany;

public static class DeleteCompanyEndpoint
{
    public static IEndpointRouteBuilder MapDeleteCompany(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/companies/{id:guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = new DeleteCompanyCommand(id);
            var result = await mediator.Send(command, ct);
            return Results.Ok(result);
        })
        .WithName("DeleteCompany")
        .Produces<DeleteCompanyResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}