using EmuSync.Domain.Services;

namespace EmuSync.Domain.Tests.Services;

public class LocalDataAccessorTests
{
    private static LocalDataAccessor CreateAccessor() => new();

    private static string CreateTempDirectory()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        return tempDir;
    }

    private class TestFile
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    [Fact]
    public async Task WriteFile_ReadFile_ReturnsExpectedContents()
    {
        var accessor = CreateAccessor();
        var tempDir = CreateTempDirectory();
        var file = Path.Combine(tempDir, "test.json");

        try
        {
            var obj = new TestFile { Name = "x", Value = 5 };
            await accessor.WriteFileContentsAsync(file, obj);

            Assert.True(File.Exists(file));

            var read = await accessor.ReadFileContentsAsync<TestFile>(file);
            Assert.Equal("x", read.Name);
            Assert.Equal(5, read.Value);
        }
        finally
        {
            try { Directory.Delete(tempDir, true); } catch { }
        }
    }

    [Fact]
    public async Task ReadFileOrDefault_WhenFileMissing_ReturnsNull()
    {
        var accessor = CreateAccessor();
        var file = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "nofile.json");

        var result = await accessor.ReadFileContentsOrDefaultAsync<TestFile>(file);
        Assert.Null(result);
    }

    [Fact]
    public void RemoveFile_WhenFileDoesNotExist_DoesNotThrow()
    {
        var accessor = CreateAccessor();
        var file = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "nofile.json");

        accessor.RemoveFile(file); // should not throw
    }

    [Fact]
    public void ScanDirectory_WhenNull_ReturnsDirectoryNotSet()
    {
        var accessor = CreateAccessor();
        var result = accessor.ScanDirectory(null);

        Assert.False(result.DirectoryIsSet);
    }
}