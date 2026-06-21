using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class Serpentscale : SneckoCardModel, IHasOverflowEffect
{
    public Serpentscale() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(9);
        this.WithOverflow();
        WithPower<PlatedArmorPower>(3, 1);
    }

    public Task OverflowEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<PlatedArmorPower>(ctx, this);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (SneckoCmd.OverflowActive(this)) return;
        await CommonActions.ApplySelf<PlatedArmorPower>(ctx, this, 1);
    }
}