using eCarbon.Api.Features.Companies.CreateCompany;
using FluentValidation.TestHelper;

namespace eCarbon.UnitTests.Features.Companies;

public class CreateCompanyValidatorTests
{
    private readonly CreateCompanyValidator _validator;

    public CreateCompanyValidatorTests()
    {
        _validator = new CreateCompanyValidator();
    }

    [Fact]
    public void Should_HaveError_When_NameIsEmpty()
    {
        var result = _validator.TestValidate(new CreateCompanyCommand(""));
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_HaveError_When_NameIsTooLong()
    {
        var longName = new string('A', 201);
        var result = _validator.TestValidate(new CreateCompanyCommand(longName));
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_NotHaveError_When_NameIsValid()
    {
        var result = _validator.TestValidate(new CreateCompanyCommand("Acme Corporation"));
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}
