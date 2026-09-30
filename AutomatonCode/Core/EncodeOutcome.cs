using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Automaton.AutomatonCode.Events;
using Automaton.AutomatonCode.Interfaces;

namespace Automaton.AutomatonCode.Core;

/// <summary>Why (if at all) a played card ends up in the Encode pile.</summary>
public enum EncodeKind
{
    /// <summary>The card is not encoded; it goes wherever the game would normally send it.</summary>
    None,

    /// <summary>The card has the Encode keyword (<see cref="AutomatonCmd.IsEncodable" />): its Encodings run when it is played.</summary>
    OnPlay,

    /// <summary>Another listener (<see cref="IForceEncodesCard" />, e.g. Platinum Core) encodes the card; it plays normally first.</summary>
    Forced
}

/// <summary>
///     The single owner of "what happens to this card play with respect to Encode". Everything that
///     needs the answer reads <see cref="Of" />: the before-play effect (<see cref="RunPlayEffect" />),
///     the after-play move into the Encode pile (<see cref="CommitAfterPlay" />), the Bronze/Summon Orb
///     eligibility checks (<see cref="WillEncode" />), and the destination reported to vanilla
///     Rebound (<see cref="HideFromDiscard" />, registered in <c>AutomatonMainFile</c>).
///     A card that is a dupe is still "encoded" here as far as the decision goes: its Encodings still
///     run and orbs still leave it alone, but <see cref="AutomatonCmd.EncodeCard(CardModel, PlayerChoiceContext)" />
///     skips the pile move, so the transient copy vanishes.
/// </summary>
public static class EncodeOutcome
{
    public static EncodeKind Of(CardModel card)
    {
        if (AutomatonCmd.IsEncodable(card)) return EncodeKind.OnPlay;
        var combatState = card.Owner.Creature.CombatState;
        return AutomatonHook.WillForceEncode(combatState, card) ? EncodeKind.Forced : EncodeKind.None;
    }

    public static bool WillEncode(CardModel card)
    {
        return Of(card) != EncodeKind.None;
    }

    /// <summary>Before the card's own effect: player-encodable cards express their effect through their Encodings.</summary>
    public static async Task RunPlayEffect(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (Of(card) != EncodeKind.OnPlay || card is not IEncodable encodable) return;
        foreach (var encoding in encodable.Encodings)
            await encoding.OnPlay(card, ctx, cardPlay.Target, cardPlay);
    }

    /// <summary>
    ///     After the card fully resolved (including enchantments such as Momentum, see
    ///     <see cref="AutomatonCombatModel.AfterCardPlayed" />): moves it into the Encode pile.
    /// </summary>
    public static async Task CommitAfterPlay(CardModel card, PlayerChoiceContext ctx)
    {
        if (WillEncode(card))
            await AutomatonCmd.EncodeCard(card, ctx);
    }

    /// <summary>
    ///     Initial-location filter (see <see cref="CardPlayLocationCompat.RegisterInitialLocationFilter" />):
    ///     vanilla Rebound only redirects a card that would resolve to Discard. An encoded card is about
    ///     to be moved to the Encode pile instead, so report it as "not Discard" (PileType.None, the same
    ///     way Exhaust/Power/dupe cards opt out).
    /// </summary>
    public static CardLocationCompatiblity HideFromDiscard(CardModel card, CardLocationCompatiblity location)
    {
        if (location.PileType != PileType.Discard || !WillEncode(card)) return location;
        return location with { PileType = PileType.None };
    }
}
