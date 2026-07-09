using Godot;

using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Artists;

public class ArtistHoverTip(LocString title, Texture2D? icon) : IHoverTip
{
    public LocString Title => title;
    public Texture2D? Icon => icon;
    public string Id => $"ArtistHoverTip:{title}";
    public bool IsSmart => false;
    public bool IsDebuff => false;
    public bool IsInstanced => false;
    public AbstractModel? CanonicalModel => null;
}
