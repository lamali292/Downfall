using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Events;

public interface IModifyManaburnDamage
{
    decimal ModifyManaburnDamage(decimal amount, decimal original, Player player);
    Task AfterModifyingManaburnDamage(PlayerChoiceContext ctx, Player player);
}