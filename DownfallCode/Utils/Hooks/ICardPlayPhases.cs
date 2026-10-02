using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Utils;

/// <summary>
///     The keyword mechanics a character runs around one card play (Retract/Advance, Chant, Encode, ...).
///     A character's card base exposes one of these through <c>DownfallCardModel.PlayPhases</c>, so the
///     phases run only for cards of that base instead of being broadcast to every card the way the
///     global <see cref="CardExecutionHooks" /> listeners are. Detection inside a phase stays keyword-based
///     (<c>card.Keywords.Contains(...)</c>).
///     Per-play state that has to travel from <see cref="BeforePlay" /> to <see cref="AfterPlay" /> belongs
///     on the play itself (a <c>SpireField&lt;CardPlay, T&gt;</c>), never on the card instance or a plain static.
///     Phases run on every client, so they must not depend on local-only state.
/// </summary>
public interface ICardPlayPhases
{
    /// <summary>
    ///     Runs before the card's own effect. Return <c>false</c> to CANCEL the play. Every before-phase and
    ///     every global before-listener still runs when one of them cancels, so recording state here is safe.
    /// </summary>
    Task<bool> BeforePlay(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return Task.FromResult(true);
    }

    /// <summary>Runs after the card's own effect; only reached when the play was not canceled.</summary>
    Task AfterPlay(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }
}
