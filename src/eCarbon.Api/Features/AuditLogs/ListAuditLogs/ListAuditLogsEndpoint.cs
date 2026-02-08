using MediatR;

namespace eCarbon.Api.Features.AuditLogs.ListAuditLogs;

public static class ListAuditLogsEndpoint
{
    public static IEndpointRouteBuilder MapListAuditLogs(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/audit-logs", async (
            string? entityType,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1,
            int pageSize = 50,
            IMediator? mediator = null) =>
        {
            var query = new ListAuditLogsQuery(entityType, fromDate, toDate, page, pageSize);
            var result = await mediator!.Send(query);
            return Results.Ok(result);
        })
        .WithName("ListAuditLogs")
        .WithTags("Audit Logs");

        return app;
    }
}
