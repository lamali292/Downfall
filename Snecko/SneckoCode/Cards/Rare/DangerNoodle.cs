using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class DangerNoodle : SneckoCardModel
{
    public DangerNoodle() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithGift(new Gift
        {
            MinCost = 3
        });
    }

    // TODO: Implement
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
    }
}