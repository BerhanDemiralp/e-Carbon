using FluentValidation;

namespace eCarbon.Api.Features.Reports.GenerateReport;

public class GenerateReportValidator : AbstractValidator<GenerateReportCommand>
{
    public GenerateReportValidator()
    {
        RuleFor(x => x.SnapshotId)
            .NotEmpty()
            .WithMessage("Snapshot ID is required");
    }
}
