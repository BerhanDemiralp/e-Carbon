using MediatR;

namespace eCarbon.Api.Features.Companies.GetCompany;

public record GetCompanyQuery(Guid Id) : IRequest<GetCompanyResponse>;

public record GetCompanyResponse(
    Guid Id, 
    string Name, 
    DateTime CreatedAt, 
    DateTime UpdatedAt,
    int FacilityCount);