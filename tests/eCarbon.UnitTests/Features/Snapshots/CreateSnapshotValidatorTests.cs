using eCarbon.Api.Features.Snapshots.CreateSnapshot;
using FluentValidation.TestHelper;

namespace eCarbon.UnitTests.Features.Snapshots;

public class CreateSnapshotValidatorTests
{
    private readonly CreateSnapshotValidator _validator;

    public CreateSnapshotValidatorTests()
    {
        _validator = new CreateSnapshotValidator();
    }

    [Fact]
    public void Should_HaveError_When_CompanyIdIsEmpty()
    {
        var result = _validator.TestValidate(new CreateSnapshotCommand(Guid.Empty, "2026-01"));
        result.ShouldHaveValidationErrorFor(x => x.CompanyId);
    }

    [Fact]
    public void Should_HaveError_When_MonthIsInvalidFormat()
    {
        var result = _validator.TestValidate(new CreateSnapshotCommand(Guid.NewGuid(), "invalid"));
        result.ShouldHaveValidationErrorFor(x => x.Month);
    }

    [Fact]
    public void Should_HaveError_When_MonthIsEmpty()
    {
        var result = _validator.TestValidate(new CreateSnapshotCommand(Guid.NewGuid(), ""));
        result.ShouldHaveValidationErrorFor(x => x.Month);
    }

    [Fact]
    public void Should_NotHaveError_When_InputsAreValid()
    {
        var result = _validator.TestValidate(new CreateSnapshotCommand(Guid.NewGuid(), "2026-01"));
        result.ShouldNotHaveValidationErrorFor(x => x.CompanyId);
        result.ShouldNotHaveValidationErrorFor(x => x.Month);
    }
}
