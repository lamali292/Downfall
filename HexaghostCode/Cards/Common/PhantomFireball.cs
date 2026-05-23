using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Common;

[Pool(typeof(HexaghostCardPool))]
public class PhantomFireball : HexaghostCardModel
{
    public PhantomFireball() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(4, 2);
        WithTip(typeof(SoulBurnPower));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (cardPlay.Target == null || cardPlay.Target.IsDead || !cardPlay.Target.HasPower<SoulBurnPower>()) return;
        var power = cardPlay.Target.GetPower<SoulBurnPower>();
        if (power == null) return;
        await power.Detonate(ctx, Owner.Creature, IsUpgraded);
    }
}