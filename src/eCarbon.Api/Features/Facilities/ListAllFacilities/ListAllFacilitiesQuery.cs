using MediatR;

namespace eCarbon.Api.Features.Facilities.ListAllFacilities;

public record ListAllFacilitiesQuery : IRequest<List<ListAllFacilitiesResponse>>;

public record ListAllFacilitiesResponse(
    Guid Id,
    string Name,
    string Location,
    Guid CompanyId,
    string CompanyName,
    DateTime CreatedAt,
    int ActivityRecordCount);