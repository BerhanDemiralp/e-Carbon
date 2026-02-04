using FluentValidation;

namespace eCarbon.Api.Features.Companies.CreateCompany;

public class CreateCompanyValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Şirket adı zorunludur");
        
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .WithMessage("Şirket adı en az 3 karakter olmalıdır");
        
        RuleFor(x => x.Name)
            .MaximumLength(200)
            .WithMessage("Şirket adı en fazla 200 karakter olabilir");
        
        RuleFor(x => x.Name)
            .Must(BeValidCompanyName)
            .WithMessage("Şirket adı özel karakterler içeremez");
    }
    
    private bool BeValidCompanyName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return true;
            
        // Sadece harf, rakam, boşluk ve bazı özel karakterlere izin ver
        var invalidChars = new[] { '<', '>', '&', '"', '\'' };
        return !invalidChars.Any(c => name.Contains(c));
    }
}