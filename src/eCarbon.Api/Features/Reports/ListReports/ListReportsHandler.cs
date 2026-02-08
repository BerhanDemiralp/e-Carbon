using eCarbon.Api.Common.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCarbon.Api.Features.Reports.ListReports;

public class ListReportsHandler : IRequestHandler<ListReportsQuery, List<ListReportsResponse>>
{
    private readonly AppDbContext _dbContext;

    public ListReportsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ListReportsResponse>> Handle(ListReportsQuery request, CancellationToken cancellationToken)
    {
        var reports = await _dbContext.Reports
            .Include(r => r.Snapshot)
                .ThenInclude(s => s.Company)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ListReportsResponse(
                r.Id,
                r.SnapshotId,
                r.Snapshot.Company.Name,
                r.Snapshot.Month,
                r.Snapshot.Status.ToString(),
                r.Snapshot.TotalKg,
                r.CreatedAt,
                r.HashValue
            ))
            .ToListAsync(cancellationToken);

        return reports;
    }
}
