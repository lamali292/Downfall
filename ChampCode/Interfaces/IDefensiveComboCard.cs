using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Interfaces;

public interface IDefensiveComboCard
{
    public Task DefensiveComboEffect(PlayerChoiceContext ctx, CardPlay cardPlay);
}