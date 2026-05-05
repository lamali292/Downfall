using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
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
        var mutableCards = CardFactory.GetDistinctForCombat(Owner, 
            SneckoModel.GetSneckoCards(Owner), 
            DynamicVars.Cards.IntValue,
            Owner.RunState.Rng.CombatCardGeneration).ToList();
        mutableCards.ForEach(card => card.AddKeyword(CardKeyword.Retain));
        await CardPileCmd.Add(mutableCards, PileType.Hand);
    }
}