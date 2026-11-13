using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class Overexert : SlimeBossCardModel
{
    public Overexert() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override bool HasEnergyCostX => true;

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var count = ResolveEnergyXValue() + (IsUpgraded ? 1 : 0);
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // Snapshot up front: playing cards changes the hand. OrderBy is stable, so equal costs keep hand order.
        var toPlay = Owner.Hand.Where(c => c != this)
            .OrderBy(c => c.EnergyCost.GetAmountToSpend())
            .Take(count)
            .ToList();
        foreach (var card in toPlay)
        {
            if (card.Pile?.Type != PileType.Hand) continue;
            await CardCmd.AutoPlay(ctx, card, null);
        }
    }
}
