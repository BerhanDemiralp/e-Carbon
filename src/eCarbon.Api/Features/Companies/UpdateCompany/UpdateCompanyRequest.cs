using MediatR;

namespace eCarbon.Api.Features.Companies.UpdateCompany;

public class UpdateCompanyRequest
{
    public string Name { get; set; } = string.Empty;
    
    public UpdateCompanyCommand ToCommand(Guid id) => new(id, Name);
}