using eCarbon.Api.Common.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCarbon.Api.Features.Facilities.ListAllFacilities;

public class ListAllFacilitiesHandler : IRequestHandler<ListAllFacilitiesQuery, List<ListAllFacilitiesResponse>>
{
    private readonly AppDbContext _dbContext;

    public ListAllFacilitiesHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ListAllFacilitiesResponse>> Handle(ListAllFacilitiesQuery request, CancellationToken cancellationToken)
    {
        var facilities = await _dbContext.Facilities
            .AsNoTracking()
            .Include(f => f.Company)
            .Include(f => f.ActivityRecords)
            .OrderBy(f => f.Name)
            .Select(f => new ListAllFacilitiesResponse(
                f.Id,
                f.Name,
                f.Location,
                f.CompanyId,
                f.Company.Name,
                f.CreatedAt,
                f.ActivityRecords.Count))
            .ToListAsync(cancellationToken);

        return facilities;
    }
}