using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
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
        WithTip<WeakPower>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        // Checked before the attack: an enemy it kills has already lost its Weak, but still counts.
        var hadWeak = SlimeBossCmd.HasWeak(this, cardPlay);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (!hadWeak) return;
        if (cardPlay.Target?.GetPower<WeakPower>() is { Amount: > 0 } weak)
            await PowerCmd.ModifyAmount(ctx, weak, -1, Owner.Creature, this);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }
}
