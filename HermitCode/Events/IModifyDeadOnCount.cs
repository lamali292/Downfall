using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Events;

public interface IModifyDeadOnCount
{
    int ModifyDeadOnCount(int amount, CardModel card);
    Task AfterModifyingDeadOnCount(PlayerChoiceContext ctx, CardModel card);
}