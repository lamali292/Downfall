using Downfall.DownfallCode.Events;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Relics;

/// <summary>
///     Whenever you remove or Transform a card from your deck, heal 15 HP.
/// </summary>
public sealed class StraightRazor : HermitRelicModel, IAfterCardTransformed
{
    public StraightRazor() : base(RelicRarity.Uncommon)
    {
        WithVars(new HealVar(15));
        WithTip(StaticHoverTip.Transform);
    }

    public async Task AfterCardTransformed(CardModel replacement)
    {
        if (replacement.Owner != Owner) return;
        if (replacement.Pile?.Type != PileType.Deck) return;
        Flash();
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
    }


    public override async Task BeforeCardRemoved(CardModel card)
    {
        if (card.Owner != Owner) return;
        if (card.Pile?.Type != PileType.Deck) return;
        Flash();
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
    }
}