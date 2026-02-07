namespace eCarbon.Api.Features.Facilities.CreateFacility;

public class CreateFacilityRequest
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    public CreateFacilityCommand ToCommand(Guid companyId) => new(companyId, Name, Location);
}