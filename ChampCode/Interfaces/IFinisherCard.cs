using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Interfaces;

public interface IFinisherCard
{
    bool AffectsAllPlayers { get; }
    Task FinisherEffect(PlayerChoiceContext ctx, CardPlay cardPlay);
}