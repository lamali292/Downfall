using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class SoulDraw : SneckoCardModel
{
    public SoulDraw() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithCards(2);
        WithCostUpgradeBy(-1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var mutableCards = SneckoModel.GetCombatSneckoCards(Owner, DynamicVars.Cards.IntValue).ToList();
        mutableCards.ForEach(card => card.AddKeyword(CardKeyword.Retain));
        await CardPileCmd.Add(mutableCards, PileType.Hand);
    }
}