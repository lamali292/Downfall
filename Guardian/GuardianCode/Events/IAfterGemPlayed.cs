using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Events;

public interface IAfterGemPlayed
{
    Task AfterGemPlayed(PlayerChoiceContext ctx, GemModel gemModel, CardPlay? cardPlay);
}