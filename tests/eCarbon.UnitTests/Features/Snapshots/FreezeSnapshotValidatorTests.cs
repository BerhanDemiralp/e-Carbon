using eCarbon.Api.Features.Snapshots.FreezeSnapshot;
using FluentValidation.TestHelper;

namespace eCarbon.UnitTests.Features.Snapshots;

public class FreezeSnapshotValidatorTests
{
    private readonly FreezeSnapshotValidator _validator;

    public FreezeSnapshotValidatorTests()
    {
        _validator = new FreezeSnapshotValidator();
    }

    [Fact]
    public void Should_HaveError_When_SnapshotIdIsEmpty()
    {
        var result = _validator.TestValidate(new FreezeSnapshotCommand(Guid.Empty));
        result.ShouldHaveValidationErrorFor(x => x.SnapshotId);
    }

    [Fact]
    public void Should_NotHaveError_When_SnapshotIdIsValid()
    {
        var result = _validator.TestValidate(new FreezeSnapshotCommand(Guid.NewGuid()));
        result.ShouldNotHaveValidationErrorFor(x => x.SnapshotId);
    }
}
