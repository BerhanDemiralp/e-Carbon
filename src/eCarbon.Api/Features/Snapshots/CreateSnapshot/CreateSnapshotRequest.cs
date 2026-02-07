namespace eCarbon.Api.Features.Snapshots.CreateSnapshot;

public class CreateSnapshotRequest
{
    public string Month { get; set; } = string.Empty;

    public CreateSnapshotCommand ToCommand(Guid companyId) => new(companyId, Month);
}