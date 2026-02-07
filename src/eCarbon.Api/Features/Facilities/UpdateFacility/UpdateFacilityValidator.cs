using FluentValidation;

namespace eCarbon.Api.Features.Facilities.UpdateFacility;

public class UpdateFacilityValidator : AbstractValidator<UpdateFacilityCommand>
{
    public UpdateFacilityValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Facility ID is required");

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