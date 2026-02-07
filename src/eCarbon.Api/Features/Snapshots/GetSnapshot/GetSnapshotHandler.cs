using eCarbon.Api.Common.Exceptions;
using eCarbon.Api.Common.Persistence;
using eCarbon.Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCarbon.Api.Features.Snapshots.GetSnapshot;

public class GetSnapshotHandler : IRequestHandler<GetSnapshotQuery, GetSnapshotResponse>
{
    private readonly AppDbContext _dbContext;

    public GetSnapshotHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetSnapshotResponse> Handle(GetSnapshotQuery request, CancellationToken cancellationToken)
    {
        var snapshot = await _dbContext.MonthlySnapshots
            .Include(s => s.Company)
            .Include(s => s.SnapshotItems)
                .ThenInclude(item => item.Facility)
            .FirstOrDefaultAsync(s => s.Id == request.SnapshotId, cancellationToken);

        if (snapshot == null)
        {
            throw new NotFoundException("Snapshot", request.SnapshotId);
        }

        var items = snapshot.SnapshotItems.Select(item => new SnapshotItemDto(
            item.Id,
            item.Facility?.Name ?? "Unknown Facility",
            item.ActivityDate,
            item.ActivityType.ToString(),
            (int)item.Scope,
            item.Quantity,
            item.Unit,
            item.FactorKgPerUnit,
            item.Co2eKg
        )).ToList();

        return new GetSnapshotResponse(
            snapshot.Id,
            snapshot.CompanyId,
            snapshot.Company?.Name ?? "Unknown Company",
            snapshot.Month,
            snapshot.Status.ToString(),
            snapshot.Scope1TotalKg,
            snapshot.Scope2TotalKg,
            snapshot.TotalKg,
            snapshot.CreatedAt,
            snapshot.FrozenAt,
            items
        );
    }
}
