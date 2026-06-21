using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class Recycling : SlimeBossCardModel
{
    public Recycling() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
        WithTip(SlimeBossTip.Slurp);
        this.WithPower<RecyclingPower>(1, false);
    }

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<RecyclingPower>(ctx, this);
    }
}