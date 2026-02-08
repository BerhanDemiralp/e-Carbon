using eCarbon.Api.Common.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCarbon.Api.Features.AuditLogs.ListAuditLogs;

public class ListAuditLogsHandler : IRequestHandler<ListAuditLogsQuery, ListAuditLogsResponse>
{
    private readonly AppDbContext _dbContext;

    public ListAuditLogsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ListAuditLogsResponse> Handle(ListAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.AuditLogs.AsQueryable();

        if (!string.IsNullOrEmpty(request.EntityType))
        {
            query = query.Where(x => x.EntityType == request.EntityType);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt <= request.ToDate.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var logs = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new AuditLogDto(
                x.Id,
                x.EntityType,
                x.EntityId.ToString(),
                x.Action,
                x.Actor,
                x.Summary,
                x.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return new ListAuditLogsResponse(logs, totalCount, request.Page, request.PageSize);
    }
}
