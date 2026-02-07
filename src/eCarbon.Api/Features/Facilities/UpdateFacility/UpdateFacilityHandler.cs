using eCarbon.Api.Common.Exceptions;
using eCarbon.Api.Common.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCarbon.Api.Features.Facilities.UpdateFacility;

public class UpdateFacilityHandler : IRequestHandler<UpdateFacilityCommand, UpdateFacilityResponse>
{
    private readonly AppDbContext _dbContext;

    public UpdateFacilityHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UpdateFacilityResponse> Handle(UpdateFacilityCommand request, CancellationToken cancellationToken)
    {
        var facility = await _dbContext.Facilities
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

        if (facility == null)
        {
            throw new NotFoundException("Facility", request.Id);
        }

        facility.Name = request.Name;
        facility.Location = request.Location;
        facility.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateFacilityResponse(
            facility.Id,
            facility.CompanyId,
            facility.Name,
            facility.Location,
            facility.UpdatedAt);
    }
}