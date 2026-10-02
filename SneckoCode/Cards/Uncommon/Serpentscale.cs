using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class Serpentscale : SneckoCardModel
{
    public Serpentscale() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(9);
        WithOverflow();
        WithPower<PlatedArmorPower>(2, 1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var overflowing = OverflowCmd.OverflowActive(this);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CommonActions.ApplySelf<PlatedArmorPower>(ctx, this, 1);
        await OverflowCmd.Overflow(overflowing, cardPlay, () => CommonActions.ApplySelf<PlatedArmorPower>(ctx, this));
    }
}