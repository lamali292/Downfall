using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SlimeBoss.SlimeBossCode.Interfaces;

public interface IHasConsumeEffect
{
    Task ConsumeEffect(PlayerChoiceContext ctx, CardPlay? cardPlay, Creature target);
}