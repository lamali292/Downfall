using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Powers;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class MinionMaster : SlimeBossCardModel
{
    public MinionMaster() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<MinionMasterPower>(1, false);
        WithKeyword(CardKeyword.Innate);
        WithSlimeTip<BruiserSlime>();
        WithTip(SlimeBossTip.Command);
    }

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<MinionMasterPower>(ctx, this);
    }
}
