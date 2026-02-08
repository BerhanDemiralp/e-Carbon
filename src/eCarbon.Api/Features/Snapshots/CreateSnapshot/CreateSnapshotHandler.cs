using eCarbon.Api.Common.Exceptions;
using eCarbon.Api.Common.Persistence;
using eCarbon.Api.Domain.Entities;
using eCarbon.Api.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCarbon.Api.Features.Snapshots.CreateSnapshot;

public class CreateSnapshotHandler : IRequestHandler<CreateSnapshotCommand, CreateSnapshotResponse>
{
    private readonly AppDbContext _dbContext;

    public CreateSnapshotHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateSnapshotResponse> Handle(CreateSnapshotCommand request, CancellationToken cancellationToken)
    {
        // Verify company exists
        var company = await _dbContext.Companies
            .FirstOrDefaultAsync(c => c.Id == request.CompanyId, cancellationToken);

        if (company == null)
        {
            throw new NotFoundException("Company", request.CompanyId);
        }

        // Parse month (format: YYYY-MM)
        if (!DateTime.TryParseExact(request.Month + "-01", "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var monthStart))
        {
            throw new ArgumentException("Invalid month format. Use YYYY-MM.");
        }

        // Ensure UTC
        monthStart = DateTime.SpecifyKind(monthStart, DateTimeKind.Utc);
        var monthEnd = DateTime.SpecifyKind(monthStart.AddMonths(1).AddDays(-1), DateTimeKind.Utc);

        // Validate month is not in the future
        var today = DateTime.UtcNow;
        if (monthStart > new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc))
        {
            throw new InvalidOperationException("Cannot create snapshot for a future month.");
        }

        // Check if snapshot already exists for this company and month
        var existingSnapshot = await _dbContext.MonthlySnapshots
            .FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId && s.Month == request.Month, cancellationToken);

        if (existingSnapshot != null)
        {
            throw new InvalidOperationException($"Snapshot already exists for company in month {request.Month}");
        }

        // Get all facilities for this company
        var facilityIds = await _dbContext.Facilities
            .Where(f => f.CompanyId == request.CompanyId)
            .Select(f => f.Id)
            .ToListAsync(cancellationToken);

        if (!facilityIds.Any())
        {
            throw new InvalidOperationException("Company has no facilities. Cannot create snapshot.");
        }

        // Get all activity records for these facilities in the specified month
        var activities = await _dbContext.ActivityRecords
            .AsNoTracking()
            .Include(ar => ar.Facility)
            .Where(ar => facilityIds.Contains(ar.FacilityId)
                && ar.ActivityDate >= monthStart
                && ar.ActivityDate <= monthEnd)
            .ToListAsync(cancellationToken);

        if (!activities.Any())
        {
            throw new InvalidOperationException($"No activity records found for month {request.Month}");
        }

        // Create snapshot
        var snapshot = new MonthlySnapshot
        {
            Id = Guid.NewGuid(),
            CompanyId = request.CompanyId,
            Month = request.Month,
            Status = SnapshotStatus.Draft,
            Scope1TotalKg = 0,
            Scope2TotalKg = 0,
            TotalKg = 0,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.MonthlySnapshots.Add(snapshot);

        // Create snapshot items with frozen factor values
        decimal scope1Total = 0;
        decimal scope2Total = 0;

        foreach (var activity in activities)
        {
            // Find emission factor
            var factor = await _dbContext.EmissionFactors
                .FirstOrDefaultAsync(ef => ef.ActivityType == activity.ActivityType
                    && ef.Unit == activity.Unit
                    && ef.IsActive, cancellationToken);

            if (factor == null)
            {
                continue; // Skip activities without matching factors
            }

            var co2eKg = activity.Quantity * factor.KgCo2ePerUnit;

            var snapshotItem = new SnapshotItem
            {
                Id = Guid.NewGuid(),
                SnapshotId = snapshot.Id,
                FacilityId = activity.FacilityId,
                ActivityDate = activity.ActivityDate,
                Scope = activity.Scope,
                ActivityType = activity.ActivityType,
                Quantity = activity.Quantity,
                Unit = activity.Unit,
                FactorKgPerUnit = factor.KgCo2ePerUnit, // Frozen factor value!
                Co2eKg = co2eKg,
                SourceActivityRecordId = activity.Id,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.SnapshotItems.Add(snapshotItem);

            if (activity.Scope == ScopeType.Scope1)
            {
                scope1Total += co2eKg;
            }
            else
            {
                scope2Total += co2eKg;
            }
        }

        // Update snapshot totals
        snapshot.Scope1TotalKg = scope1Total;
        snapshot.Scope2TotalKg = scope2Total;
        snapshot.TotalKg = scope1Total + scope2Total;

        await _dbContext.SaveChangesAsync(cancellationToken);

        var itemCount = activities.Count;

        return new CreateSnapshotResponse(
            snapshot.Id,
            snapshot.CompanyId,
            snapshot.Month,
            snapshot.Status.ToString(),
            snapshot.Scope1TotalKg,
            snapshot.Scope2TotalKg,
            snapshot.TotalKg,
            snapshot.CreatedAt,
            itemCount);
    }
}