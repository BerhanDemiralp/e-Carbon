using eCarbon.Api.Common.Exceptions;
using eCarbon.Api.Common.Persistence;
using eCarbon.Api.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCarbon.Api.Features.Snapshots.FreezeSnapshot;

public class FreezeSnapshotHandler : IRequestHandler<FreezeSnapshotCommand, FreezeSnapshotResponse>
{
    private readonly AppDbContext _dbContext;

    public FreezeSnapshotHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FreezeSnapshotResponse> Handle(FreezeSnapshotCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await _dbContext.MonthlySnapshots
            .FirstOrDefaultAsync(s => s.Id == request.SnapshotId, cancellationToken);

        if (snapshot == null)
        {
            throw new NotFoundException("Snapshot", request.SnapshotId);
        }

        if (snapshot.Status == SnapshotStatus.Frozen)
        {
            throw new InvalidOperationException("Snapshot is already frozen");
        }

        snapshot.Status = SnapshotStatus.Frozen;
        snapshot.FrozenAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new FreezeSnapshotResponse(
            snapshot.Id,
            snapshot.CompanyId,
            snapshot.Month,
            snapshot.Status.ToString(),
            snapshot.Scope1TotalKg,
            snapshot.Scope2TotalKg,
            snapshot.TotalKg,
            snapshot.CreatedAt,
            snapshot.FrozenAt.Value
        );
    }
}
