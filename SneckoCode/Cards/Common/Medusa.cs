using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class Medusa : SneckoCardModel, IHasGift
{
    public Medusa() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        this.WithGift(new Gift
        {
            Rarity = CardRarity.Common,
            IsDebuff = true
        });
        WithDamage(7, 2);
        WithPower<VenomPower>(2, 1);
    }

    public Gift? Gift { get; set; }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CommonActions.Apply<VenomPower>(ctx, this, cardPlay);
    }
}