using Downfall.DownfallCode.Compatibility;
using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gremlins.GremlinsCode.Powers;

public class FuriousPower() : GremlinsPowerModel(PowerType.Buff, PowerStackType.Single)
{
    public override async Task AfterBlockGained(Creature creature, decimal amount,
        ValueProp props,
        CardModel? cardSource)
    {
        if (creature != Owner || amount <= 0 || Owner.Player == null) return;
        var ctx = new BlockingPlayerChoiceContext();
        await CompatibilityCreatureCmd.LoseBlock(ctx, Owner, amount, Owner);
        var attack = DamageCmd.Attack(amount);
        attack.Attacker = Owner;
        attack._attackerAnimName = "Attack";
        attack._attackerAnimDelay = Owner.Player.Character.AttackAnimDelay;
        attack.ModelSource = this;
        attack._sourceType = AttackCommand.SourceType.Card;
        attack.TargetingAllOpponents(CombatState);
        await attack.Execute(ctx);
    }
}