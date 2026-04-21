using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Interfaces;

public interface ITickCard
{
    Task OnTick(PlayerChoiceContext ctx);
}