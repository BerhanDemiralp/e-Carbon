using eCarbon.Api.Domain.Entities;
using eCarbon.Api.Common.Persistence;
using MediatR;
using System.Security.Claims;

namespace eCarbon.Api.Common.Behaviors;

public class AuditingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditingBehavior(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();
        
        // Only audit commands (requests that modify data)
        var requestName = typeof(TRequest).Name;
        if (requestName.EndsWith("Command"))
        {
            await AuditActionAsync(requestName, request, cancellationToken);
        }

        return response;
    }

    private async Task AuditActionAsync(string actionName, object request, CancellationToken ct)
    {
        var user = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";
        
        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            Actor = user,
            Action = actionName.Replace("Command", ""),
            EntityType = request.GetType().Name.Replace("Command", ""),
            EntityId = ExtractEntityId(request),
            Summary = $"Executed {actionName}",
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.AuditLogs.Add(auditLog);
        await _dbContext.SaveChangesAsync(ct);
    }

    private static Guid ExtractEntityId(object request)
    {
        // Try to extract Id from request (simplified)
        var idProperty = request.GetType().GetProperty("Id");
        if (idProperty != null)
        {
            var value = idProperty.GetValue(request);
            if (value is Guid guid)
            {
                return guid;
            }
        }
        return Guid.Empty;
    }
}