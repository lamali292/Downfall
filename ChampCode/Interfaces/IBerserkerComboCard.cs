using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Interfaces;

public interface IBerserkerComboCard
{
    public Task BerserkerComboEffect(PlayerChoiceContext ctx, CardPlay cardPlay);
}