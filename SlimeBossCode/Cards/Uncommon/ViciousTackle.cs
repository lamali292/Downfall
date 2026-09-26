using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class ViciousTackle : SlimeBossCardModel
{
    public ViciousTackle() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithTags(SlimeBossTag.Tackle);
        WithDamage(10, 4);
        WithKeyword(CardKeyword.Exhaust);
        WithTip<WeakPower>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var target = cardPlay.Target;
        if (target == null) return;
        var powerAmount = target.IsAlive ? target.GetPowerAmount<WeakPower>() : 0;
        if (powerAmount <= 0) return;
        await PowerCmd.Apply<WeakPower>(ctx, target, powerAmount, Owner.Creature, this);
    }
}
