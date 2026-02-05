using MediatR;

namespace eCarbon.Api.Features.Companies.DeleteCompany;

public record DeleteCompanyCommand(Guid Id) : IRequest<DeleteCompanyResponse>;

public record DeleteCompanyResponse(
    Guid Id, 
    string Message);