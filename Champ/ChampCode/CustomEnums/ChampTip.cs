using Downfall.DownfallCode.Abstract;

namespace Champ.ChampCode.CustomEnums;

public class ChampTip(string name) : CustomStaticTip(name)
{
    public static readonly ChampTip Finisher = new(nameof(Finisher));
}