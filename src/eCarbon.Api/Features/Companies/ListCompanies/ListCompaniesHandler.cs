using eCarbon.Api.Common.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCarbon.Api.Features.Companies.ListCompanies;

public class ListCompaniesHandler : IRequestHandler<ListCompaniesQuery, List<ListCompaniesResponse>>
{
    private readonly AppDbContext _dbContext;

    public ListCompaniesHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ListCompaniesResponse>> Handle(ListCompaniesQuery request, CancellationToken cancellationToken)
    {
        var companies = await _dbContext.Companies
            .AsNoTracking()
            .Include(c => c.Facilities)
            .OrderBy(c => c.Name)
            .Select(c => new ListCompaniesResponse(
                c.Id,
                c.Name,
                c.CreatedAt,
                c.Facilities.Count))
            .ToListAsync(cancellationToken);

        return companies;
    }
}