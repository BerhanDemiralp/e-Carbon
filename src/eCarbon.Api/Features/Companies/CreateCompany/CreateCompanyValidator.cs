using FluentValidation;

namespace eCarbon.Api.Features.Companies.CreateCompany;

public class CreateCompanyValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Company name is required");
        
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .WithMessage("Company name must be at least 3 characters");
        
        RuleFor(x => x.Name)
            .MaximumLength(200)
            .WithMessage("Company name cannot exceed 200 characters");
        
        RuleFor(x => x.Name)
            .Must(BeValidCompanyName)
            .WithMessage("Company name cannot contain special characters");
    }
    
    private bool BeValidCompanyName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return true;
            
        // Allow only letters, numbers, spaces and some special characters
        var invalidChars = new[] { '<', '>', '&', '"', '\'' };
        return !invalidChars.Any(c => name.Contains(c));
    }
}