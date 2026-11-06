namespace Downfall.DownfallCode.Utils;

public static partial class ReplaceableSubmod
{
    /// <summary>
    /// Pure check: is any id in <paramref name="replacementModIds"/> present in
    /// <paramref name="loadedModIds"/>? Exact id match only - no substring/prefix matching.
    /// No game/BaseLib usings here so this half of the type can be unit-tested in
    /// isolation (see UnitTests/ReplaceableSubmodTests.cs).
    /// </summary>
    public static bool IsSupersededByAny(IEnumerable<string?> loadedModIds, IEnumerable<string> replacementModIds)
    {
        var replacementSet = new HashSet<string>(replacementModIds);
        return loadedModIds.Any(id => id != null && replacementSet.Contains(id));
    }
}
