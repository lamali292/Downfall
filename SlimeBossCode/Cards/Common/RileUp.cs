using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Powers;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class RileUp : SlimeBossCardModel
{
    public RileUp() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(6, 1);
        WithPower<RileUpPotencyPower>(2, 2, false);
        WithTip<PotencyPower>();
        WithSlimeTip<BruiserSlime>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var slime = Owner.GetSlime<BruiserSlime>();
        if (slime == null) return;
        await CommonActions.Apply<RileUpPotencyPower>(ctx, slime, this);
    }
}

public class RileUpPotencyPower : CustomTemporaryPowerModelWrapper<RileUp, PotencyPower>;
