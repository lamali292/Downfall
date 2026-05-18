using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Relics;

/// <summary>
///     Whenever you remove or Transform a card from your deck, heal 15 HP.
/// </summary>
public sealed class StraightRazor : HermitRelicModel
{
    public StraightRazor() : base(RelicRarity.Uncommon)
    {
        WithVars(new HealVar(15));
        WithTip(StaticHoverTip.Transform);
    }


    public override async Task BeforeCardRemoved(CardModel card)
    {
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
    }
}