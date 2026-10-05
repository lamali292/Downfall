using Downfall.DownfallCode.Utils;
using Xunit;

namespace Downfall.UnitTests;

public class ReplaceableSubmodTests
{
    [Fact]
    public void NoReplacementLoaded_IsNotSuperseded()
    {
        var loaded = new[] { "Downfall", "SlimeBoss" };
        var replacements = new[] { "SlimeBossBeta" };

        Assert.False(ReplaceableSubmod.IsSupersededByAny(loaded, replacements));
    }

    [Fact]
    public void DeclaredReplacementLoaded_IsSuperseded()
    {
        var loaded = new[] { "Downfall", "SlimeBoss", "SlimeBossBeta" };
        var replacements = new[] { "SlimeBossBeta" };

        Assert.True(ReplaceableSubmod.IsSupersededByAny(loaded, replacements));
    }

    [Fact]
    public void UnrelatedModLoaded_IsNotSuperseded()
    {
        var loaded = new[] { "Downfall", "SomeOtherMod" };
        var replacements = new[] { "SlimeBossBeta" };

        Assert.False(ReplaceableSubmod.IsSupersededByAny(loaded, replacements));
    }

    [Fact]
    public void MatchingIsExact_NotSubstring()
    {
        // "SlimeBoss" loaded should not falsely satisfy a check for "SlimeBossBeta",
        // and vice versa - this must be exact id equality, not Contains/StartsWith.
        Assert.False(ReplaceableSubmod.IsSupersededByAny(new[] { "SlimeBoss" }, new[] { "SlimeBossBeta" }));
        Assert.False(ReplaceableSubmod.IsSupersededByAny(new[] { "SlimeBossBeta" }, new[] { "SlimeBoss" }));
    }

    [Fact]
    public void MultipleReplacementCandidates_AnyMatchSupersedes()
    {
        var loaded = new[] { "Downfall", "CollectorBeta" };
        var replacements = new[] { "SlimeBossBeta", "CollectorBeta" };

        Assert.True(ReplaceableSubmod.IsSupersededByAny(loaded, replacements));
    }

    [Fact]
    public void NullManifestId_IsIgnored()
    {
        // ModManager.GetLoadedMods() entries can have a null manifest id in practice
        // (manifest?.id) - must not throw or false-positive on that.
        var loaded = new string?[] { null, "SlimeBoss" };

        Assert.False(ReplaceableSubmod.IsSupersededByAny(loaded, new[] { "SlimeBossBeta" }));
    }
}
