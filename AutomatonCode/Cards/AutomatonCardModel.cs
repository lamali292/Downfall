using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.DynamicVars;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Extensions;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Automaton.AutomatonCode.Cards;

public abstract class
    AutomatonCardModel : DownfallCardModel<Core.Automaton>
{
    protected AutomatonCardModel(
        int cost,
        CardType type,
        CardRarity rarity,
        TargetType targetType,
        bool showInCardLibrary = true,
        bool autoAdd = true
    ) : base(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
    {
        if (AutomatonCmd.IsEncodable(this))
            WithTip(AutomatonTip.Encode);
        if (this is ICompilable)
            WithTip(AutomatonTip.Compile);
    }
    
    
    protected void WithStash(int baseValue, int upgradeValue = 0)
    {
        WithVars(new StashVar(baseValue).WithUpgrade(upgradeValue));
    }
}