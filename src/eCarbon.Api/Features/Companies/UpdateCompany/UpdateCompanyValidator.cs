using FluentValidation;

namespace eCarbon.Api.Features.Companies.UpdateCompany;

public class UpdateCompanyValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Company ID is required");
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Company name is required");
        
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .WithMessage("Company name must be at least 3 characters");
        
        RuleFor(x => x.Name)
            .MaximumLength(200)
            .WithMessage("Company name cannot exceed 200 characters");
    }
}