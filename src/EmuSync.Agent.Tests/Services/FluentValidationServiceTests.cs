using EmuSync.Agent.Services;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace EmuSync.Agent.Tests.Services;

public class FluentValidationServiceTests
{
    private static FluentValidationService CreateSut(IServiceProvider? sp = null)
        => new(sp ?? new Mock<IServiceProvider>().Object);

    [Fact]
    public async Task ValidateAsync_WhenModelIsNull_ReturnsError()
    {
        var sut = CreateSut();
        var validator = new Mock<IValidator<object>>().Object;

        var result = await sut.ValidateAsync<object>(null!, validator);

        Assert.Single(result);
        Assert.Equal("Received an unexpected null value", result[0]);
    }

    [Fact]
    public async Task ValidateAsync_WhenValidatorProvided_UsesIt()
    {
        var validator = new Mock<IValidator<string>>();
        validator
            .Setup(v => v.ValidateAsync("x", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(
                new[] { new ValidationFailure("field", "err") }
            ));

        var sut = CreateSut();

        var result = await sut.ValidateAsync("x", validator.Object);

        Assert.Single(result);
        Assert.Equal("err", result[0]);
    }

    [Fact]
    public async Task ValidateAsync_WhenValidatorNotProvided_ResolvesFromServiceProvider()
    {
        var validator = new Mock<IValidator<int>>();
        validator
            .Setup(v => v.ValidateAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var sp = new Mock<IServiceProvider>();
        sp.Setup(x => x.GetService(typeof(IValidator<int>)))
          .Returns(validator.Object);

        var sut = CreateSut(sp.Object);

        var result = await sut.ValidateAsync(1);

        Assert.Empty(result);
    }

    [Fact]
    public void ValidateIdsMatch_WhenIdsMatch_ReturnsEmpty()
    {
        var sut = CreateSut();

        var result = sut.ValidateIdsMatch("a", "a");

        Assert.Empty(result);
    }

    [Fact]
    public void ValidateIdsMatch_WhenIdsDoNotMatch_ReturnsErrorMessage()
    {
        var sut = CreateSut();

        var result = sut.ValidateIdsMatch("a", "b", "m");

        Assert.Single(result);
        Assert.Equal("m", result[0]);
    }
}