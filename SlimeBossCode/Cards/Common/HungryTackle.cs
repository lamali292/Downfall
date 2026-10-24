using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class HungryTackle : SlimeBossCardModel
{
    public HungryTackle() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(12, 1);
        WithEnergy(3, 1);
        WithTags(SlimeBossTag.Tackle);
        WithKeyword(CardKeyword.Exhaust);
        WithTip(SlimeBossTip.Consume);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var weakTargets = SlimeBossCmd.WeakTargets(this, cardPlay);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await SlimeBossCmd.Consume(ctx, this, weakTargets,
            _ => PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner));
    }
}
