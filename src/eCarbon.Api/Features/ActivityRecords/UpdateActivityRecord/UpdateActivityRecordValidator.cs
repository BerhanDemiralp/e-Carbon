using eCarbon.Api.Domain.Enums;
using FluentValidation;

namespace eCarbon.Api.Features.ActivityRecords.UpdateActivityRecord;

public class UpdateActivityRecordValidator : AbstractValidator<UpdateActivityRecordCommand>
{
    public UpdateActivityRecordValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Activity record ID is required");

        RuleFor(x => x.ActivityDate)
            .NotEmpty()
            .WithMessage("Activity date is required");

        RuleFor(x => x.ActivityDate)
            .LessThanOrEqualTo(DateTime.Now)
            .WithMessage("Activity date cannot be in the future");

        RuleFor(x => x.Scope)
            .IsInEnum()
            .WithMessage("Scope must be 1 (Scope 1) or 2 (Scope 2)");

        RuleFor(x => x.ActivityType)
            .IsInEnum()
            .WithMessage("Invalid activity type");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.Unit)
            .NotEmpty()
            .WithMessage("Unit is required");

        RuleFor(x => x.Unit)
            .MaximumLength(20)
            .WithMessage("Unit cannot exceed 20 characters");
    }
}