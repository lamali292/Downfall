using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class ButcherTackle : SlimeBossCardModel
{
    public ButcherTackle() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithTags(SlimeBossTag.Tackle);
        WithDamage(20, 5);
        WithTip(SlimeBossTip.Consume);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await SlimeBossCmd.Consume(ctx, this, cardPlay,
            async _ =>
            {
                await CommonActions.ApplySelf<ShadowStepPower>(ctx, this, 1);
                await CommonActions.ApplySelf<SlimeDoubleDamageNextTurnPower>(ctx, this, 1);
            });
    }
}
