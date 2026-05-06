using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class SerpentIdol : SneckoCardModel
{
    public SerpentIdol() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithCards(3);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var mutableCards = SneckoModel.GetCombatSneckoCards(Owner, DynamicVars.Cards.IntValue).ToList();
        var selectedCard = await CardSelectCmd.FromChooseACardScreen(ctx, mutableCards, Owner);
        if (selectedCard == null) return;

        selectedCard.SetToFreeThisTurn();
        await CardPileCmd.Add(selectedCard, PileType.Hand);
    }
}