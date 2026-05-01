using Downfall.DownfallCode.Abstract;

namespace Automaton.AutomatonCode.CustomEnums;

public class AutomatonTip(string name) : CustomStaticTip(name)
{
    public static readonly AutomatonTip Encode = new(nameof(Encode));
    public static readonly AutomatonTip Compile = new(nameof(Compile));
    public static readonly AutomatonTip Cycle = new(nameof(Cycle));
    public static readonly AutomatonTip Insert = new(nameof(Insert));
}