using eCarbon.Api.Features.ActivityRecords.CreateActivityRecord;
using eCarbon.Api.Domain.Enums;
using FluentValidation.TestHelper;

namespace eCarbon.UnitTests.Features.ActivityRecords;

public class CreateActivityRecordValidatorTests
{
    private readonly CreateActivityRecordValidator _validator;

    public CreateActivityRecordValidatorTests()
    {
        _validator = new CreateActivityRecordValidator();
    }

    [Fact]
    public void Should_HaveError_When_FacilityIdIsEmpty()
    {
        var command = new CreateActivityRecordCommand(
            Guid.Empty,
            DateTime.UtcNow.AddDays(-1),
            ScopeType.Scope2,
            ActivityType.Electricity,
            100,
            "kWh");

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FacilityId);
    }

    [Fact]
    public void Should_HaveError_When_QuantityIsNegative()
    {
        var command = new CreateActivityRecordCommand(
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(-1),
            ScopeType.Scope2,
            ActivityType.Electricity,
            -100,
            "kWh");

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public void Should_HaveError_When_QuantityIsZero()
    {
        var command = new CreateActivityRecordCommand(
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(-1),
            ScopeType.Scope2,
            ActivityType.Electricity,
            0,
            "kWh");

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public void Should_HaveError_When_ActivityDateIsInFuture()
    {
        var command = new CreateActivityRecordCommand(
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            ScopeType.Scope2,
            ActivityType.Electricity,
            100,
            "kWh");

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ActivityDate);
    }

    [Fact]
    public void Should_NotHaveError_When_InputsAreValid()
    {
        var command = new CreateActivityRecordCommand(
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(-1),
            ScopeType.Scope2,
            ActivityType.Electricity,
            100,
            "kWh");

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.FacilityId);
        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
        result.ShouldNotHaveValidationErrorFor(x => x.ActivityDate);
    }
}
