using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class GoopArmor : SlimeBossCardModel
{
    public GoopArmor() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<GoopArmorPower>(4, 2, false);
        WithTip(StaticHoverTip.Block);
    }

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<GoopArmorPower>(ctx, this);
    }
}
