using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Powers;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class Mafioso : SlimeBossCardModel
{
    public Mafioso() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithPower<MafiosoPower>(1, false);
        WithSlimeTip<BruiserSlime>();
        WithPower<PotencyPower>(2);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MafiosoPower>(ctx, this);
        var slime = Owner.GetSlime<BruiserSlime>();
        if (slime == null) return;
        await CommonActions.Apply<PotencyPower>(ctx, slime, this);
    }
}
