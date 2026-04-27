using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Events;

public interface IBeforeCardEntersStasis
{
    Task BeforeCardEntersStasis(PlayerChoiceContext ctx, CardModel card, AbstractModel source);
}