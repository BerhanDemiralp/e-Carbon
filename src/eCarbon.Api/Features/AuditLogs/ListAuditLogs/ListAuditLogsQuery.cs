using MediatR;

namespace eCarbon.Api.Features.AuditLogs.ListAuditLogs;

public record ListAuditLogsQuery(
    string? EntityType = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int Page = 1,
    int PageSize = 50
) : IRequest<ListAuditLogsResponse>;

public record ListAuditLogsResponse(
    List<AuditLogDto> Logs,
    int TotalCount,
    int Page,
    int PageSize
);

public record AuditLogDto(
    Guid Id,
    string EntityType,
    string EntityId,
    string Action,
    string Actor,
    string Summary,
    DateTime CreatedAt);
