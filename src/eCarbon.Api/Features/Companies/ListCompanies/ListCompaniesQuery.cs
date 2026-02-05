using MediatR;

namespace eCarbon.Api.Features.Companies.ListCompanies;

public record ListCompaniesQuery : IRequest<List<ListCompaniesResponse>>;

public record ListCompaniesResponse(
    Guid Id, 
    string Name, 
    DateTime CreatedAt,
    int FacilityCount);