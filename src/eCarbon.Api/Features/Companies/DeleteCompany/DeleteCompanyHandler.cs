using eCarbon.Api.Common.Exceptions;
using eCarbon.Api.Common.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCarbon.Api.Features.Companies.DeleteCompany;

public class DeleteCompanyHandler : IRequestHandler<DeleteCompanyCommand, DeleteCompanyResponse>
{
    private readonly AppDbContext _dbContext;

    public DeleteCompanyHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeleteCompanyResponse> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _dbContext.Companies
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (company == null)
        {
            throw new NotFoundException("Company", request.Id);
        }

        // Soft delete - IsDeleted flag'ini true yap
        company.IsDeleted = true;
        company.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new DeleteCompanyResponse(
            company.Id,
            "Şirket başarıyla silindi (soft delete)");
    }
}