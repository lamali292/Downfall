using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class SnakeEyes : SneckoCardModel
{
    public SnakeEyes() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithGift(new Gift
        {
            Rarity = CardRarity.Rare,
            Type = CardType.Skill
        });
    }

    // TODO: Implement
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
    }
}