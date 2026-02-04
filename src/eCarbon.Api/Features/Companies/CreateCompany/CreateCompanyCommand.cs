using MediatR;

namespace eCarbon.Api.Features.Companies.CreateCompany;

public record CreateCompanyCommand(string Name) : IRequest<CreateCompanyResponse>;

public record CreateCompanyResponse(Guid Id, string Name, DateTime CreatedAt);