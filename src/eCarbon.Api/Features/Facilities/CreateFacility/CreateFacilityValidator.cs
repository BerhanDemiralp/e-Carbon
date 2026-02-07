using FluentValidation;

namespace eCarbon.Api.Features.Facilities.CreateFacility;

public class CreateFacilityValidator : AbstractValidator<CreateFacilityCommand>
{
    public CreateFacilityValidator()
    {
        RuleFor(x => x.CompanyId)
            .NotEmpty()
            .WithMessage("Company ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Facility name is required");

        RuleFor(x => x.Name)
            .MinimumLength(3)
            .WithMessage("Facility name must be at least 3 characters");

        RuleFor(x => x.Name)
            .MaximumLength(200)
            .WithMessage("Facility name cannot exceed 200 characters");

        RuleFor(x => x.Location)
            .NotEmpty()
            .WithMessage("Location is required");

        RuleFor(x => x.Location)
            .MaximumLength(300)
            .WithMessage("Location cannot exceed 300 characters");
    }
}