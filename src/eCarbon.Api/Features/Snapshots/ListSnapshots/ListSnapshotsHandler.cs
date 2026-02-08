using eCarbon.Api.Common.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCarbon.Api.Features.Snapshots.ListSnapshots;

public class ListSnapshotsHandler : IRequestHandler<ListSnapshotsQuery, List<ListSnapshotsResponse>>
{
    private readonly AppDbContext _dbContext;

    public ListSnapshotsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ListSnapshotsResponse>> Handle(ListSnapshotsQuery request, CancellationToken cancellationToken)
    {
        var snapshots = await _dbContext.MonthlySnapshots
            .Include(s => s.Company)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new ListSnapshotsResponse(
                s.Id,
                s.CompanyId,
                s.Company.Name,
                s.Month,
                s.Status.ToString(),
                s.Scope1TotalKg,
                s.Scope2TotalKg,
                s.TotalKg,
                s.CreatedAt,
                s.FrozenAt,
                s.SnapshotItems.Count
            ))
            .ToListAsync(cancellationToken);

        return snapshots;
    }
}
