using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SlimeBoss.SlimeBossCode.Events;

public interface IAfterConsumeEffect
{
    Task AfterConsumeEffect(PlayerChoiceContext ctx, Creature creature, Creature attacker, decimal goopAmount);
}