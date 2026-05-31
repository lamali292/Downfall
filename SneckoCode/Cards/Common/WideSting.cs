using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class WideSting : SneckoCardModel, IHasGift
{
    public WideSting() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        this.WithGift(new Gift
        {
            Rarity = CardRarity.Common
        });
        WithDamage(7, 3);
    }

    protected override Artist Artist => Artist.Get<Magerblutooth>();

    public Gift? Gift { get; set; }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        foreach (var card in Owner.GetHand()
                     .Where(e => e.IsUpgradable && SneckoCmd.IsOffclass(this, e)))
            CardCmd.Upgrade(card);
    }
}