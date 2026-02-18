using EmuSync.Services.Storage.SharedFolder;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace EmuSync.Services.Storage;


public class NetworkConnection : IDisposable
{
    private readonly string _networkName;

    public NetworkConnection(SharedFolderDetails details)
    {
        _networkName = details.Path;

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        var netResource = new NetResource()
        {
            Scope = ResourceScope.GlobalNetwork,
            ResourceType = ResourceType.Disk,
            DisplayType = ResourceDisplaytype.Share,
            RemoteName = details.Path,
            LocalName = null!,
            Provider = null!
        };

        var result = WNetAddConnection2(
            netResource,
            details.Password!,
            details.Username!,
            0
        );

        if (result != 0)
        {
            throw new Win32Exception(result);
        }
    }

    ~NetworkConnection()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        WNetCancelConnection2(_networkName, 0, true);
    }

    [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
    private static extern int WNetAddConnection2(
        NetResource netResource,
        string password,
        string username,
        int flags);

    [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
    private static extern int WNetCancelConnection2(
        string name,
        int flags,
        bool force);
}

[StructLayout(LayoutKind.Sequential)]
public class NetResource
{
    public ResourceScope Scope;
    public ResourceType ResourceType;
    public ResourceDisplaytype DisplayType;
    public int Usage;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string LocalName;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string RemoteName;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string Comment;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string Provider;
}

public enum ResourceScope : int
{
    Connected = 1,
    GlobalNetwork,
    Remembered,
    Recent,
    Context
};

public enum ResourceType : int
{
    Any = 0,
    Disk = 1,
    Print = 2,
    Reserved = 8,
}

public enum ResourceDisplaytype : int
{
    Generic = 0x0,
    Domain = 0x01,
    Server = 0x02,
    Share = 0x03,
    File = 0x04,
    Group = 0x05,
    Network = 0x06,
    Root = 0x07,
    Shareadmin = 0x08,
    Directory = 0x09,
    Tree = 0x0a,
    Ndscontainer = 0x0b
}