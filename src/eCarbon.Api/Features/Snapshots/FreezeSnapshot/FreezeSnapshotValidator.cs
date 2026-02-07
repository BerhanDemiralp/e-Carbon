using FluentValidation;

namespace eCarbon.Api.Features.Snapshots.FreezeSnapshot;

public class FreezeSnapshotValidator : AbstractValidator<FreezeSnapshotCommand>
{
    public FreezeSnapshotValidator()
    {
        RuleFor(x => x.SnapshotId)
            .NotEmpty()
            .WithMessage("Snapshot ID is required");
    }
}
