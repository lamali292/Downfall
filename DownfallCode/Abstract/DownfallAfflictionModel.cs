using BaseLib.Extensions;

namespace Downfall.DownfallCode.Abstract;

public abstract class DownfallAfflictionModel<T> : CustomAfflictionModel
    where T : DownfallCharacterModel
{
    public override string CustomOverlayPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.tscn".AfflictionScenePath<T>();
}