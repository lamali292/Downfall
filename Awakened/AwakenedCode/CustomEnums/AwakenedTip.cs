using BaseLib.Utils;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.CustomEnums;

public class AwakenedTip(string name)  : CustomStaticTip(name)
{
    public static readonly AwakenedTip Conjure = new(nameof(Conjure));
    public static readonly AwakenedTip Chant = new(nameof(Chant));
    public static readonly AwakenedTip Drained = new(nameof(Drained));
}

