using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Interfaces;
using Downfall.DownfallCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Core;

/// <summary>
///     Collector's play phases: Pyre / Megapyre are paid before the card's effect (the play is canceled when
///     nothing can be pyred) and the pyred cards are released after it.
///     The pyred cards are still stored on the card (<see cref="IUsesPyredCards.PyredCards" />), not on the
///     play: JadedJabs reads them from its calculated-damage callback, which only receives the card.
/// </summary>
public sealed class CollectorCardPlayPhases : ICardPlayPhases
{
    public static readonly CollectorCardPlayPhases Instance = new();

    public async Task<bool> BeforePlay(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (card.Keywords.Contains(CollectorKeyword.Pyre))
        {
            var pyred = await CollectorCmd.Pyre(ctx, card);
            if (card is IUsesPyredCards pyre) pyre.PyredCards = pyred == null ? [] : [pyred];
            return pyred != null;
        }

        if (card.Keywords.Contains(CollectorKeyword.Megapyre))
        {
            var pyred = await CollectorCmd.MegaPyre(ctx, card);
            if (card is IUsesPyredCards pyre) pyre.PyredCards = pyred;
            return pyred.Any();
        }

        return true;
    }

    public Task AfterPlay(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (card is IUsesPyredCards pyre) pyre.PyredCards = [];
        return Task.CompletedTask;
    }
}
