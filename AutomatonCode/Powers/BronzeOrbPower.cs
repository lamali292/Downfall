using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Piles;
using Automaton.AutomatonCode.Vfx;
using Downfall.DownfallCode.Compatibility;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Powers;

public class BronzeOrbPower : AutomatonPowerModel, IModifyCardPlayResultLocation
{
    //// todo make fail if stash is full and still tick down, make tick down even if the card has encode etc
    public CardLocationCompatiblity ModifyCardPlayResultLocationCompability(CardModel card, bool isAutoPlay,
        ResourceInfo resources, CardLocationCompatiblity cardLocation)
    {
        if (!IsCardWeWant(card)) return cardLocation;
        // Bottom = background, matching every other stash entry point (StashCmd.Run ->
        // CardPileCmd.Add's default position). Top would put it in front of whatever's
        // already stashed, which looks wrong for a pile that's meant to queue in order.
        return new CardLocationCompatiblity(card.Owner, StashPile.Stash, CardPilePosition.Bottom);
    }


    public Task AfterModifyingCardPlayResultLocationCompability(CardModel card, CardLocationCompatiblity cardLocation)
    {
        // Bronze Orb redirects the card's play-result location directly instead of going through
        // StashCmd.Run (the "one and only stash flow"), so it has to reveal the Stash pile button
        // itself - otherwise the first stash of the whole combat, when the pile button starts
        // hidden (NStashPile.StartHidden is true for anyone not playing Automaton, e.g. a
        // Snecko/borrowed-pool Bronze Orb), never gets revealed and the button never appears.
        if (LocalContext.IsMe(card.Owner))
            Callable.From(() => NStashPile.RevealFor(card.Owner)).CallDeferred();
        return PowerCmd.Decrement(this);
    }

    private bool IsCardWeWant(CardModel card)
    {
        var player = card.Owner;
        return player.Creature == Owner &&
               card.Type is CardType.Attack or CardType.Skill &&
               !card.Keywords.Contains(CardKeyword.Exhaust) &&
               !AutomatonCmd.WillAutoEncode(card);
    }


    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Side) return Task.CompletedTask;
        PowerCmd.Remove(this);
        return Task.CompletedTask;
    }
}