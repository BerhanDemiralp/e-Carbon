using eCarbon.Api.Common.Exceptions;
using eCarbon.Api.Common.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCarbon.Api.Features.Companies.GetCompany;

public class GetCompanyHandler : IRequestHandler<GetCompanyQuery, GetCompanyResponse>
{
    private readonly AppDbContext _dbContext;

    public GetCompanyHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetCompanyResponse> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
    {
        var company = await _dbContext.Companies
            .AsNoTracking()
            .Include(c => c.Facilities)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (company == null)
        {
            throw new NotFoundException("Company", request.Id);
        }

        return new GetCompanyResponse(
            company.Id,
            company.Name,
            company.CreatedAt,
            company.UpdatedAt,
            company.Facilities.Count);
    }
}