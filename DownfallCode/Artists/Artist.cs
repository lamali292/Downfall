using BaseLib.Extensions;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace Downfall.DownfallCode.Artists;

public abstract class Artist
{
    private static readonly Dictionary<Type, Artist> Instances = new();

    public static T Get<T>() where T : Artist, new()
        => (T)(Instances.TryGetValue(typeof(T), out var a) ? a : Instances[typeof(T)] = new T());

    private string Id => GetType().GetPrefix()+StringHelper.Slugify(GetType().Name);
    private static LocString ArtByLocString => new("artists", "ART_BY");
    private LocString Name => new("artists", $"{Id}.name");
    private LocString ArtByName
    {
        get
        {
            var text = ArtByLocString;
            text.Add("name", Name.GetFormattedText());
            return text;
        }
    }

    private Texture2D? Icon => IconPath == null ? null : ResourceLoader.Load<Texture2D>(IconPath);
    protected virtual string? IconPath => $"{Id}.png".ArtistImagePath();
    public IHoverTip HoverTip => new ShiftOnlyHoverTip(new HoverTip(ArtByName, Icon));
}


public class GoofballMcgee : Artist;
public class Eudaimonia : Artist;
public class Opal : Artist;
public class Occultpyromancer : Artist;

public interface IShiftOnlyHoverTip : IHoverTip { }

public class ShiftOnlyHoverTip(HoverTip inner) : IShiftOnlyHoverTip
{
    public HoverTip Inner => inner;
    public string Id => inner.Id;
    public bool IsSmart => inner.IsSmart;
    public bool IsDebuff => inner.IsDebuff;
    public bool IsInstanced => inner.IsInstanced;
    public AbstractModel? CanonicalModel => inner.CanonicalModel;
}




[HarmonyPatch(typeof(NHoverTipSet), "Init")]
static class NHoverTipSetInitPatch
{
    static void Prefix(ref IEnumerable<IHoverTip> hoverTips)
    {
        var shiftHeld = Input.IsPhysicalKeyPressed(Key.Shift);
        hoverTips = hoverTips
            .Where(tip => tip is not ShiftOnlyHoverTip || shiftHeld)
            .Select(tip => tip is ShiftOnlyHoverTip s ? s.Inner : tip)
            .ToList();
    }
}