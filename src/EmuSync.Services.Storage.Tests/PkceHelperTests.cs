namespace EmuSync.Services.Storage.Tests;

public class PkceHelperTests
{
    [Fact]
    public void GenerateCodeVerifier_DefaultLength()
    {
        var v = PkceHelper.GenerateCodeVerifier();
        Assert.Equal(64, v.Length);
        Assert.Matches(@"^[a-zA-Z0-9\-\._~]+$", v);
    }

    [Fact]
    public void GenerateCodeVerifier_CustomLength()
    {
        var v = PkceHelper.GenerateCodeVerifier(32);
        Assert.Equal(32, v.Length);
    }

    [Fact]
    public void CreateCodeChallenge_Produces_UrlSafe_Base64()
    {
        var verifier = PkceHelper.GenerateCodeVerifier();
        var challenge = PkceHelper.CreateCodeChallenge(verifier);
        Assert.False(string.IsNullOrEmpty(challenge));
        Assert.DoesNotContain("=", challenge);
        Assert.Matches(@"^[A-Za-z0-9_-]+$", challenge);
    }
}
