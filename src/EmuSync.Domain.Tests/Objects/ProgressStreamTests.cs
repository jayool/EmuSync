using EmuSync.Domain.Objects;

namespace EmuSync.Domain.Tests.Objects;

public class ProgressStreamTests
{
    [Fact]
    public void Read_Reports_Progress()
    {
        byte[] data = new byte[100];
        for (int i = 0; i < data.Length; i++) data[i] = (byte)i;

        using var ms = new MemoryStream(data);
        double last = -1;
        using var ps = new ProgressStream(ms, p => last = p, (ulong)data.Length);

        byte[] buffer = new byte[25];
        int read;
        while ((read = ps.Read(buffer, 0, buffer.Length)) > 0)
        {
            // loop
        }

        Assert.True(last >= 100);
    }

    [Fact]
    public async Task ReadAsync_Reports_Progress()
    {
        byte[] data = new byte[50];
        using var ms = new MemoryStream(data);
        double last = -1;
        using var ps = new ProgressStream(ms, p => last = p, (ulong)data.Length);

        byte[] buffer = new byte[10];
        int read;
        while ((read = await ps.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            // loop
        }

        Assert.True(last >= 100);
    }

    [Fact]
    public void Write_Reports_Progress()
    {
        using var ms = new MemoryStream();
        double last = -1;
        using var ps = new ProgressStream(ms, p => last = p, 10);

        byte[] buf = new byte[5];
        ps.Write(buf, 0, buf.Length);
        ps.Write(buf, 0, buf.Length);

        Assert.True(last >= 100);
    }

    [Fact]
    public async Task WriteAsync_Reports_Progress()
    {
        using var ms = new MemoryStream();
        double last = -1;
        using var ps = new ProgressStream(ms, p => last = p, 4);

        byte[] buf = new byte[2];
        await ps.WriteAsync(buf, 0, buf.Length, CancellationToken.None);
        await ps.WriteAsync(buf, 0, buf.Length, CancellationToken.None);

        Assert.True(last >= 100);
    }
}
