using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Multiplayer;

[Pool(typeof(SneckoCardPool))]
public class Sssharing : SneckoCardModel
{
    public Sssharing() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AllAllies)
    {
        WithCards(1, 1);
        WithKeyword(CardKeyword.Exhaust);
        WithTip(CardKeyword.Retain);
    }
    

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var amount = DynamicVars.Cards.IntValue;
        foreach (var player in Owner.RunState.Players)
        {
            var pool = SneckoModel.GetCombatSneckoCards(Owner, amount, player)
                .ToList();
            foreach (var card in pool)
            {
                card.SetToFreeThisTurn();
            }
            await CardPileCmd.Add(pool, PileType.Hand);
        }
    }
}