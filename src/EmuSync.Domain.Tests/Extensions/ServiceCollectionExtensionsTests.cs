using EmuSync.Domain.Extensions;
using EmuSync.Domain.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmuSync.Domain.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddLocalDataAccessor_Registers_Service()
    {
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder().Build();

        services.AddLocalDataAccessor(config);

        var sp = services.BuildServiceProvider();

        var svc = sp.GetService<ILocalDataAccessor>();
        Assert.NotNull(svc);
    }
}
