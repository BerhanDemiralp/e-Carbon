using eCarbon.Api.Common.Exceptions;
using eCarbon.Api.Common.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCarbon.Api.Features.Facilities.GetFacility;

public class GetFacilityHandler : IRequestHandler<GetFacilityQuery, GetFacilityResponse>
{
    private readonly AppDbContext _dbContext;

    public GetFacilityHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetFacilityResponse> Handle(GetFacilityQuery request, CancellationToken cancellationToken)
    {
        var facility = await _dbContext.Facilities
            .AsNoTracking()
            .Include(f => f.ActivityRecords)
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

        if (facility == null)
        {
            throw new NotFoundException("Facility", request.Id);
        }

        return new GetFacilityResponse(
            facility.Id,
            facility.CompanyId,
            facility.Name,
            facility.Location,
            facility.CreatedAt,
            facility.UpdatedAt,
            facility.ActivityRecords.Count);
    }
}