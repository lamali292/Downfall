using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Interfaces;

public interface IHasConsumeEffect
{
    Task ConsumeEffect(PlayerChoiceContext ctx, Creature creature, AttackCommand command, int amount);
}