using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class MinionMaster : SlimeBossCardModel
{
    public MinionMaster() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<MinionMasterPower>(1, false);
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
    }

    protected override Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<MinionMasterPower>(ctx, this);
    }
}