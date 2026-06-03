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

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var exhaustOnePrefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
        var card = (await CardSelectCmd.FromHand(ctx, Owner, exhaustOnePrefs, e => e != this, this))
            .FirstOrDefault();
        if (card == null) return;
        await CardCmd.Exhaust(ctx, card);
        if (!ModelDb.AllCharacterCardPools.Contains(card.Pool)) return;
        // TODO : Probably make a restriction here, lol
        var nopeCard = Owner.RunState.Rng.CombatCardGeneration.NextItem(card.Pool.AllCards);
        if (nopeCard == null) return;
        var combatCard = CombatState!.CreateCard(nopeCard, Owner);
        await CardPileCmd.AddGeneratedCardToCombat(combatCard, PileType.Hand, Owner);
    }
}