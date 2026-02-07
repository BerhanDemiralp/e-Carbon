using MediatR;

namespace eCarbon.Api.Features.Facilities.ListFacilitiesByCompany;

public record ListFacilitiesByCompanyQuery(Guid CompanyId) : IRequest<List<ListFacilitiesByCompanyResponse>>;

public record ListFacilitiesByCompanyResponse(
    Guid Id,
    string Name,
    string Location,
    DateTime CreatedAt,
    int ActivityRecordCount);