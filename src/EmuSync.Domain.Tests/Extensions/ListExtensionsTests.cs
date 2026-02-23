using EmuSync.Domain.Extensions;

namespace EmuSync.Domain.Tests.Extensions;

public class ListExtensionsTests
{
    [Fact]
    public void AddOrReplaceItem_Adds_WhenNotExists()
    {
        var list = new List<string> { "a" };
        list.AddOrReplaceItem("b", x => x == "b");
        Assert.Contains("b", list);
    }

    [Fact]
    public void AddOrReplaceItem_Replaces_WhenExists()
    {
        var list = new List<string> { "a", "b" };
        list.AddOrReplaceItem("bb", x => x == "b");
        Assert.Contains("bb", list);
        Assert.DoesNotContain("b", list);
    }

    [Fact]
    public void RemoveBy_Removes_WhenExists()
    {
        var list = new List<int> { 1, 2, 3 };
        list.RemoveBy(x => x == 2);
        Assert.DoesNotContain(2, list);
    }

    [Fact]
    public void RemoveBy_DoesNothing_WhenNotExists()
    {
        var list = new List<int> { 1, 3 };
        list.RemoveBy(x => x == 2);
        Assert.Equal(2, list.Count);
    }
}