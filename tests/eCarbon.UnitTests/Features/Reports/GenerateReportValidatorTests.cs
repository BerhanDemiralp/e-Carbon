using eCarbon.Api.Features.Reports.GenerateReport;
using FluentValidation.TestHelper;

namespace eCarbon.UnitTests.Features.Reports;

public class GenerateReportValidatorTests
{
    private readonly GenerateReportValidator _validator;

    public GenerateReportValidatorTests()
    {
        _validator = new GenerateReportValidator();
    }

    [Fact]
    public void Should_HaveError_When_SnapshotIdIsEmpty()
    {
        var result = _validator.TestValidate(new GenerateReportCommand(Guid.Empty));
        result.ShouldHaveValidationErrorFor(x => x.SnapshotId);
    }

    [Fact]
    public void Should_NotHaveError_When_SnapshotIdIsValid()
    {
        var result = _validator.TestValidate(new GenerateReportCommand(Guid.NewGuid()));
        result.ShouldNotHaveValidationErrorFor(x => x.SnapshotId);
    }
}
