using EmuSync.Agent.Dto.Auth;
using EmuSync.Agent.Mapping;
using EmuSync.Services.Storage.SharedFolder;

namespace EmuSync.Agent.Tests.Mapping;

public class AuthMappingsTests
{
    [Fact]
    public void SharedFolderAuthFinishDto_MapsTo_SharedFolderDetails()
    {
        var dto = new SharedFolderAuthFinishDto
        {
            Path = "\\\\server\\share",
            Username = "user",
            Password = "pass"
        };

        SharedFolderDetails details = dto.ToSharedFolderDetails();

        Assert.Equal(dto.Path, details.Path);
        Assert.Equal(dto.Username, details.Username);
        Assert.Equal(dto.Password, details.Password);
    }
}
