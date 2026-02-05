using eCarbon.Api.Common.Persistence;
using eCarbon.Api.Domain.Entities;
using MediatR;

namespace eCarbon.Api.Features.Companies.CreateCompany;

public class CreateCompanyHandler : IRequestHandler<CreateCompanyCommand, CreateCompanyResponse>
{
    private readonly AppDbContext _dbContext;

    public CreateCompanyHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateCompanyResponse> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        // Validation is already done by ValidationBehavior!
        // Here is only the business logic
        
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        _dbContext.Companies.Add(company);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CreateCompanyResponse(
            company.Id, 
            company.Name, 
            company.CreatedAt);
    }
}