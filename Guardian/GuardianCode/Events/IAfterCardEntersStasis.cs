using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Events;

public interface IAfterCardEntersStasis
{
    Task AfterCardEntersStasis(PlayerChoiceContext ctx, CardModel card, AbstractModel source);
}