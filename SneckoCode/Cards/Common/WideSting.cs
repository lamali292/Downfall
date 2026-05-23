using BaseLib.Utils;
using Downfall.DownfallCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class WideSting : SneckoCardModel
{
    public WideSting() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        WithGift(new Gift
        {
            Rarity = CardRarity.Common
        });
        WithDamage(7, 3);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        foreach (var card in Owner.GetHand()
                     .Where(e => e.IsUpgradable && SneckoCmd.IsOffclass(this, e)))
            CardCmd.Upgrade(card);
    }
}