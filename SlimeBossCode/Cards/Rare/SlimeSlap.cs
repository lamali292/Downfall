using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class SlimeSlap : SlimeBossCardModel
{
    public SlimeSlap() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithPower<DouseInSlimePower>(1);
        WithDamage(8);
        WithCostUpgradeBy(-1);
        this.WithTip<GoopPower>();
    }
    
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<DouseInSlimePower>(ctx, this, cardPlay);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}