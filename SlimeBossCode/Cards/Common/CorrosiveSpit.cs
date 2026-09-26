using BaseLib.Utils;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class CorrosiveSpit : SlimeBossCardModel
{
    public CorrosiveSpit() : base(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithPower<WeakPower>(1);
        WithHpLoss(3, 3);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<WeakPower>(ctx, this, cardPlay);
        if (cardPlay.Target == null) return;
        await CompatibilityCreatureCmd.Damage(ctx, cardPlay.Target, DynamicVars.HpLoss.BaseValue,
            DamageProps.nonCardHpLoss, Owner.Creature, this, cardPlay);
    }
}
