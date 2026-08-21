using Hermit.HermitCode.Cards.Basic;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Relics;


public sealed class Horseshoe() : HermitRelicModel(RelicRarity.Rare)
{
    public override Task BeforeAttack(AttackCommand command)
    {
        if (command.Attacker != Owner.Creature) return Task.CompletedTask;
        if (command.ModelSource is not CardModel card) return Task.CompletedTask;
        if (card is StrikeHermit) return Task.CompletedTask;
        if (!card.Tags.Contains(CardTag.Strike) || card.Rarity != CardRarity.Basic) return Task.CompletedTask;
        command._singleTarget = null;
        command._combatState = Owner.Creature.CombatState;
        command.IsRandomlyTargeted = false;
        return Task.CompletedTask;
    }
}