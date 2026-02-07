using eCarbon.Api.Common.Exceptions;
using eCarbon.Api.Common.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCarbon.Api.Features.Facilities.ListFacilitiesByCompany;

public class ListFacilitiesByCompanyHandler : IRequestHandler<ListFacilitiesByCompanyQuery, List<ListFacilitiesByCompanyResponse>>
{
    private readonly AppDbContext _dbContext;

    public ListFacilitiesByCompanyHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ListFacilitiesByCompanyResponse>> Handle(ListFacilitiesByCompanyQuery request, CancellationToken cancellationToken)
    {
        // Verify company exists
        var companyExists = await _dbContext.Companies
            .AnyAsync(c => c.Id == request.CompanyId, cancellationToken);

        if (!companyExists)
        {
            throw new NotFoundException("Company", request.CompanyId);
        }

        var facilities = await _dbContext.Facilities
            .AsNoTracking()
            .Include(f => f.ActivityRecords)
            .Where(f => f.CompanyId == request.CompanyId)
            .OrderBy(f => f.Name)
            .Select(f => new ListFacilitiesByCompanyResponse(
                f.Id,
                f.Name,
                f.Location,
                f.CreatedAt,
                f.ActivityRecords.Count))
            .ToListAsync(cancellationToken);

        return facilities;
    }
}