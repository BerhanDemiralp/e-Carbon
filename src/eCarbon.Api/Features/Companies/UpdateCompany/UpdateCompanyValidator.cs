using FluentValidation;

namespace eCarbon.Api.Features.Companies.UpdateCompany;

public class UpdateCompanyValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Şirket ID'si zorunludur");
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Şirket adı zorunludur");
        
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .WithMessage("Şirket adı en az 3 karakter olmalıdır");
        
        RuleFor(x => x.Name)
            .MaximumLength(200)
            .WithMessage("Şirket adı en fazla 200 karakter olabilir");
    }
}