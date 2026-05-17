using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class Nope : SneckoCardModel
{
    public Nope() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(7, 3);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var exhaustOnePrefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
        var card = (await CardSelectCmd.FromHand(ctx, Owner, exhaustOnePrefs, e => e != this, this))
            .FirstOrDefault();
        if (card == null) return;
        await CardCmd.Exhaust(ctx, card);
        if (!ModelDb.AllCharacterCardPools.Contains(card.Pool)) return;
        var nopeCard = card.Pool.AllCards.TakeRandom(1, Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
        if (nopeCard == null) return;
        var combatCard = CombatState!.CreateCard(nopeCard, Owner);
        await CardPileCmd.AddGeneratedCardToCombat(combatCard, PileType.Hand, Owner);
    }
}