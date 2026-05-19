using BaseLib.Utils;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Rare;

public sealed class OverwhelmingPower : HermitCardModel
{
    public OverwhelmingPower() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithCards(3, 1);
        WithEnergy(3);
        WithPower<OverwhelmingPowerPower>(4, -1);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
        await CommonActions.Draw(this, ctx);
        await CommonActions.ApplySelf<OverwhelmingPowerPower>(ctx, this);
    }
}