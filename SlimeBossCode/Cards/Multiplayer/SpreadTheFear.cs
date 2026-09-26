using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Multiplayer;

[Pool(typeof(SlimeBossCardPool))]
public class SpreadTheFear : SlimeBossCardModel
{
    public SpreadTheFear() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<SpreadTheFearPower>(1, false);
        WithTip(StaticHoverTip.Block);
        WithTip<WeakPower>();
    }
    
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;


    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<SpreadTheFearPower>(ctx, this);
    }
}
