namespace eCarbon.Api.Features.Facilities.UpdateFacility;

public class UpdateFacilityRequest
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    public UpdateFacilityCommand ToCommand(Guid id) => new(id, Name, Location);
}