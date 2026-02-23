using EmuSync.Domain.Services;

namespace EmuSync.Domain.Tests.Services;

public class SyncProgressTrackerTests
{
    [Fact]
    public void UpdateStage_SetsCurrentStage_WhenCalled()
    {
        var sut = new SyncProgressTracker();

        sut.UpdateStage("g1", "stage1");

        var progress = sut.Get("g1");

        Assert.NotNull(progress);
        Assert.Equal("stage1", progress.CurrentStage);
    }

    [Fact]
    public void UpdateStageCompletionPercent_CalculatesOverall_WhenCalled()
    {
        var sut = new SyncProgressTracker();

        sut.UpdateStageCompletionPercent("g1", 50, 0, 100);

        var progress = sut.Get("g1");

        Assert.NotNull(progress);
        Assert.Equal(50, progress.OverallCompletionPercent);
    }

    [Fact]
    public void Remove_RemovesEntry_WhenCalled()
    {
        var sut = new SyncProgressTracker();

        sut.UpdateStage("g1", "s");
        sut.Remove("g1");

        Assert.Null(sut.Get("g1"));
    }
}