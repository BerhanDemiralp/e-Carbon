using MediatR;

namespace eCarbon.Api.Features.Calculations.PreviewMonthlyEmissions;

public static class PreviewMonthlyEmissionsEndpoint
{
    public static IEndpointRouteBuilder MapPreviewMonthlyEmissions(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/companies/{companyId:guid}/months/{month}/preview", async (
            Guid companyId,
            string month,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var query = new PreviewMonthlyEmissionsQuery(companyId, month);
            var result = await mediator.Send(query, ct);
            return Results.Ok(result);
        })
        .WithName("PreviewMonthlyEmissions")
        .WithTags("Calculations");

        return app;
    }
}