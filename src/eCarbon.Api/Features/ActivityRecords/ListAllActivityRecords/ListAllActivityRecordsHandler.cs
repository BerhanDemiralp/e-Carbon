using eCarbon.Api.Common.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCarbon.Api.Features.ActivityRecords.ListAllActivityRecords;

public class ListAllActivityRecordsHandler : IRequestHandler<ListAllActivityRecordsQuery, List<ListAllActivityRecordsResponse>>
{
    private readonly AppDbContext _dbContext;

    public ListAllActivityRecordsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ListAllActivityRecordsResponse>> Handle(ListAllActivityRecordsQuery request, CancellationToken cancellationToken)
    {
        var records = await _dbContext.ActivityRecords
            .AsNoTracking()
            .Include(ar => ar.Facility)
            .ThenInclude(f => f.Company)
            .OrderByDescending(ar => ar.ActivityDate)
            .Select(ar => new ListAllActivityRecordsResponse(
                ar.Id,
                ar.FacilityId,
                ar.Facility.Name,
                ar.Facility.CompanyId,
                ar.Facility.Company.Name,
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