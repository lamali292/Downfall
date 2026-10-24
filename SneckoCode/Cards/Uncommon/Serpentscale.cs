using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class Serpentscale : SneckoCardModel
{
    public Serpentscale() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(9);
        WithOverflow();
        WithPower<PlatingPower>(2);
        WithVar("Additional", 1, 1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var overflowing = OverflowCmd.OverflowActive(this);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var amount = DynamicVars.Power<PlatingPower>().BaseValue;
        await OverflowCmd.Overflow(overflowing, cardPlay, () => Task.FromResult(amount += DynamicVars["Additional"].BaseValue));
        await CommonActions.ApplySelf<PlatingPower>(ctx, this, amount);
    }
}