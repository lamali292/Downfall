using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.DynamicVars;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

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
        if (this is not IEncodable) return;
        WithTip(AutomatonTip.Encode);
    }
    
    
    protected void WithStash(int baseValue, int upgradeValue = 0)
    {
        WithVars(new StashVar(baseValue).WithUpgrade(upgradeValue));
    }
}