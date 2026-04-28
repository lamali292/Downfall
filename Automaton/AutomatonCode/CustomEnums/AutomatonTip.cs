using BaseLib.Utils;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.CustomEnums;

public class AutomatonTip(string name)  : CustomStaticTip(name)
{
    public static readonly AutomatonTip Encode = new(nameof(Encode));
    public static readonly AutomatonTip Compile = new(nameof(Compile));
    public static readonly AutomatonTip Cycle = new(nameof(Cycle));
    public static readonly AutomatonTip Insert = new(nameof(Insert));
}
