using eCarbon.Api.Common.Exceptions;
using eCarbon.Api.Common.Persistence;
using eCarbon.Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCarbon.Api.Features.Facilities.CreateFacility;

public class CreateFacilityHandler : IRequestHandler<CreateFacilityCommand, CreateFacilityResponse>
{
    private readonly AppDbContext _dbContext;

    public CreateFacilityHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateFacilityResponse> Handle(CreateFacilityCommand request, CancellationToken cancellationToken)
    {
        // Verify company exists
        var companyExists = await _dbContext.Companies
            .AnyAsync(c => c.Id == request.CompanyId, cancellationToken);

        if (!companyExists)
        {
            throw new NotFoundException("Company", request.CompanyId);
        }

        var facility = new Facility
        {
            Id = Guid.NewGuid(),
            CompanyId = request.CompanyId,
            Name = request.Name,
            Location = request.Location,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        _dbContext.Facilities.Add(facility);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CreateFacilityResponse(
            facility.Id,
            facility.CompanyId,
            facility.Name,
            facility.Location,
            facility.CreatedAt);
    }
}