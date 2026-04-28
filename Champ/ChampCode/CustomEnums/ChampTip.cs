using BaseLib.Utils;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.CustomEnums; 

public class ChampTip(string name)  : CustomStaticTip(name)
{
    public static readonly ChampTip Finisher = new(nameof(Finisher));
}




