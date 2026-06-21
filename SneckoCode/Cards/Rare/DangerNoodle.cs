using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class DangerNoodle : SneckoCardModel, IHasGift
{
    public DangerNoodle() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        this.WithGift(new Gift
        {
            MinCost = 3
        });
        WithDamage(14, 4);
        this.WithMuddle(1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public Gift? Gift { get; set; }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await SneckoCmd.MuddleHandCards(ctx, this);
    }
}