using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Cards.Common;

[Pool(typeof(AwakenedCardPool))]
public class Clutch : AwakenedCardModel
{
    public Clutch() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(8, 3);
        WithEnergyTip();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override bool ShouldGlowRedInternal => !ZeroCostCandidates.Any();

    // Snecko Eye (and anything else that changes a card's cost) must be reflected here: this has
    // to check the card's actual current cost, not its canonical/printed one, or the glow and the
    // pick can disagree about what's really 0-cost right now.
    private IEnumerable<CardModel> ZeroCostCandidates =>
        Owner.DrawPile.Where(c => c.EnergyCost.GetAmountToSpend() == 0 && !c.EnergyCost.CostsX);

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var candidates = ZeroCostCandidates.ToList();
        if (candidates.Count == 0) return;
        var card = Owner.RunState.Rng.CombatCardSelection.NextItem(candidates)!;
        await CardPileCmd.Add(card, PileType.Hand);
    }
}