using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Interfaces;

public interface ITickCard
{
    Task OnTick(PlayerChoiceContext ctx);
}