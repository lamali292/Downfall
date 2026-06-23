using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Powers;

public class ScopeOutPower() : HermitPowerModel(PowerType.Buff, PowerStackType.Single)
{

    public override Task BeforeAttack(AttackCommand command)
    {
        if (command.Attacker != Owner || !(command.ModelSource is CardModel card && card.Tags.Contains(CardTag.Strike) && card.Rarity == CardRarity.Basic))  return Task.CompletedTask;
        command._singleTarget = null;
        command._combatState = CombatState;
        command.IsRandomlyTargeted = false;
        return Task.CompletedTask;
    }
}