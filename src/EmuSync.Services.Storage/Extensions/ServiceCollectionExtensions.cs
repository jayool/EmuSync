using EmuSync.Services.Storage.Dropbox;
using EmuSync.Services.Storage.GoogleDrive;
using EmuSync.Services.Storage.Interfaces;
using EmuSync.Services.Storage.OneDrive;
using EmuSync.Services.Storage.SharedFolder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmuSync.Services.Storage.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the services and config for all external storage providers
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    public static void AddAllExternalStorageProviders(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IStorageProviderFactory, StorageProviderFactory>();

        services.AddGoogleDriveStorageProvider(config);
        services.AddDropboxStorageProvider(config);
        services.AddOneDriveStorageProvider(config);
        services.AddSharedFolderStorageProvider(config);
    }

    /// <summary>
    /// Registers the services and config for <see cref="GoogleDriveStorageProvider"/>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    public static void AddGoogleDriveStorageProvider(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient<GoogleAuthHandler>();

        services.AddSingleton<GoogleDriveStorageProviderCache>();

        services.Configure<GoogleDriveStorageProviderConfig>(
            config.GetSection(GoogleDriveStorageProviderConfig.Section)
        );

        services.AddSingleton<GoogleDriveStorageProvider>();
        services.AddSingleton<GoogleAuthHandler>();
    }

    /// <summary>
    /// Registers the services and config for <see cref="DropboxStorageProvider"/>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    public static void AddDropboxStorageProvider(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient<DropboxAuthHandler>();

        services.Configure<DropboxStorageProviderConfig>(
            config.GetSection(DropboxStorageProviderConfig.Section)
        );

        services.AddSingleton<DropboxStorageProvider>();
        services.AddSingleton<DropboxAuthHandler>();
    }

    /// <summary>
    /// Registers the services and config for <see cref="OneDriveStorageProvider"/>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    public static void AddOneDriveStorageProvider(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient<MicrosoftAuthHandler>();

        services.Configure<OneDriveStorageProviderConfig>(
            config.GetSection(OneDriveStorageProviderConfig.Section)
        );

        services.AddHttpClient<OneDriveStorageProvider>(config =>
        {
            config.Timeout = Timeout.InfiniteTimeSpan;
        });

        services.AddSingleton<OneDriveStorageProvider>();
        services.AddSingleton<MicrosoftAuthHandler>();
    }

    /// <summary>
    /// Registers the services and config for <see cref="OneDriveStorageProvider"/>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    public static void AddSharedFolderStorageProvider(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<SharedFolderStorageProvider>();
        services.AddSingleton<SharedFolderAuthHandler>();
    }
}