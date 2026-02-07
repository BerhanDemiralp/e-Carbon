using FluentValidation;

namespace eCarbon.Api.Features.Snapshots.CreateSnapshot;

public class CreateSnapshotValidator : AbstractValidator<CreateSnapshotCommand>
{
    public CreateSnapshotValidator()
    {
        RuleFor(x => x.CompanyId)
            .NotEmpty()
            .WithMessage("Company ID is required");

        RuleFor(x => x.Month)
            .NotEmpty()
            .WithMessage("Month is required");

        RuleFor(x => x.Month)
            .Matches(@"^\d{4}-\d{2}$")
            .WithMessage("Month must be in YYYY-MM format");
    }
}