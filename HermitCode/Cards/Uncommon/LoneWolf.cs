using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

/// <summary>
///     Choose a card in your hand. It costs 0 this turn. Discard the rest of your hand.
///     Upgrade: Cost reduced from 1 to 0.
/// </summary>
public sealed class LoneWolf : HermitCardModel
{
    public LoneWolf() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
        WithCostUpgradeBy(-1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var handPile = PileType.Hand.GetPile(Owner);
        if (handPile.Cards.Count == 0) return;
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1);
        var chosen = (await CardSelectCmd.FromHand(ctx, Owner, prefs, null, this)).FirstOrDefault();
        if (chosen == null) return;
        chosen.SetToFreeThisTurn();
        var toDiscard = handPile.Cards.Where(c => c != chosen).ToList();
        await CardPileCmd.Add(toDiscard, PileType.Discard);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Uncommon
 *   usings updated
 *   OnUpgrade: migrated lines stripped, remainder kept
 *   constructor: WithCostUpgradeBy(-1)
 */