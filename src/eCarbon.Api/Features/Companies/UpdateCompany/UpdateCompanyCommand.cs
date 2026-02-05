using MediatR;

namespace eCarbon.Api.Features.Companies.UpdateCompany;

public record UpdateCompanyCommand(Guid Id, string Name) : IRequest<UpdateCompanyResponse>;

public record UpdateCompanyResponse(
    Guid Id, 
    string Name, 
    DateTime UpdatedAt);