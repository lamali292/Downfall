using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class Mafioso : SlimeBossCardModel
{
    public Mafioso() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithPower<MafiosoPower>(1, false);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MafiosoPower>(ctx, this);
        await PowerCmd.Apply<PotencyPower>(ctx, Owner.Creature, 2, Owner.Creature, this);
    }
}
