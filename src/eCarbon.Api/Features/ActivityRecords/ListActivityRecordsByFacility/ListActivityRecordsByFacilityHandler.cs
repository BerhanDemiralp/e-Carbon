using eCarbon.Api.Common.Exceptions;
using eCarbon.Api.Common.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCarbon.Api.Features.ActivityRecords.ListActivityRecordsByFacility;

public class ListActivityRecordsByFacilityHandler : IRequestHandler<ListActivityRecordsByFacilityQuery, List<ListActivityRecordsByFacilityResponse>>
{
    private readonly AppDbContext _dbContext;

    public ListActivityRecordsByFacilityHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ListActivityRecordsByFacilityResponse>> Handle(ListActivityRecordsByFacilityQuery request, CancellationToken cancellationToken)
    {
        // Verify facility exists
        var facilityExists = await _dbContext.Facilities
            .AnyAsync(f => f.Id == request.FacilityId, cancellationToken);

        if (!facilityExists)
        {
            throw new NotFoundException("Facility", request.FacilityId);
        }

        var records = await _dbContext.ActivityRecords
            .AsNoTracking()
            .Where(ar => ar.FacilityId == request.FacilityId)
            .OrderByDescending(ar => ar.ActivityDate)
            .Select(ar => new ListActivityRecordsByFacilityResponse(
                ar.Id,
                ar.ActivityDate,
                ar.ActivityType.ToString(),
                (int)ar.Scope,
                ar.Quantity,
                ar.Unit,
                ar.CreatedAt))
            .ToListAsync(cancellationToken);

        return records;
    }
}