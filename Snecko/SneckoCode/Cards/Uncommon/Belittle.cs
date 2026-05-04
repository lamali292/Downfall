using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class Belittle : SneckoCardModel
{
    public Belittle() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithGift(new Gift
        {
            Rarity = CardRarity.Uncommon,
            IsDebuff = true,
        });
    }

    // TODO: Implement
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
    }
}