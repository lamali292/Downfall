using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class DuplicatedForm : SlimeBossCardModel
{
    public DuplicatedForm() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<DuplicatedFormPower>(1, false);
        this.WithPower<EnergizedPower>(0, 1, false);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<EnergizedPower>(ctx, this);
        await CommonActions.ApplySelf<DuplicatedFormPower>(ctx, this);
    }
}