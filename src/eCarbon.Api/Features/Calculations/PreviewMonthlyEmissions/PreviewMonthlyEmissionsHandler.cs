using eCarbon.Api.Common.Exceptions;
using eCarbon.Api.Common.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCarbon.Api.Features.Calculations.PreviewMonthlyEmissions;

public class PreviewMonthlyEmissionsHandler : IRequestHandler<PreviewMonthlyEmissionsQuery, PreviewMonthlyEmissionsResponse>
{
    private readonly AppDbContext _dbContext;

    public PreviewMonthlyEmissionsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PreviewMonthlyEmissionsResponse> Handle(PreviewMonthlyEmissionsQuery request, CancellationToken cancellationToken)
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

        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        // Get all facilities for this company
        var facilityIds = await _dbContext.Facilities
            .Where(f => f.CompanyId == request.CompanyId)
            .Select(f => f.Id)
            .ToListAsync(cancellationToken);

        if (!facilityIds.Any())
        {
            // No facilities, return empty preview
            return new PreviewMonthlyEmissionsResponse(
                request.CompanyId,
                company.Name,
                request.Month,
                0,
                0,
                0,
                new List<EmissionsBreakdownItem>());
        }

        // Get all activity records for these facilities in the specified month
        var activities = await _dbContext.ActivityRecords
            .AsNoTracking()
            .Include(ar => ar.Facility)
            .Where(ar => facilityIds.Contains(ar.FacilityId)
                && ar.ActivityDate >= monthStart
                && ar.ActivityDate <= monthEnd)
            .OrderBy(ar => ar.ActivityDate)
            .ToListAsync(cancellationToken);

        if (!activities.Any())
        {
            // No activities, return empty preview
            return new PreviewMonthlyEmissionsResponse(
                request.CompanyId,
                company.Name,
                request.Month,
                0,
                0,
                0,
                new List<EmissionsBreakdownItem>());
        }

        // Calculate emissions for each activity
        var breakdown = new List<EmissionsBreakdownItem>();
        decimal scope1Total = 0;
        decimal scope2Total = 0;

        foreach (var activity in activities)
        {
            // Find matching emission factor
            var factor = await _dbContext.EmissionFactors
                .FirstOrDefaultAsync(ef => ef.ActivityType == activity.ActivityType
                    && ef.Unit == activity.Unit
                    && ef.IsActive, cancellationToken);

            if (factor == null)
            {
                // Skip activities without matching emission factors
                continue;
            }

            var co2eKg = activity.Quantity * factor.KgCo2ePerUnit;

            breakdown.Add(new EmissionsBreakdownItem(
                activity.Id,
                activity.Facility.Name,
                activity.ActivityDate,
                activity.ActivityType.ToString(),
                (int)activity.Scope,
                activity.Quantity,
                activity.Unit,
                factor.KgCo2ePerUnit,
                co2eKg));

            if (activity.Scope == Domain.Enums.ScopeType.Scope1)
            {
                scope1Total += co2eKg;
            }
            else
            {
                scope2Total += co2eKg;
            }
        }

        return new PreviewMonthlyEmissionsResponse(
            request.CompanyId,
            company.Name,
            request.Month,
            scope1Total,
            scope2Total,
            scope1Total + scope2Total,
            breakdown);
    }
}