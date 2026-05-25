using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Powers;

public class LibraryPower : AutomatonPowerModel
{
    protected override async Task AfterSideTurnStart(PlayerChoiceContext ctx, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side || Owner.Player == null) return;
        var player = Owner.Player;
        var rng = CombatState.RunState.Rng.CombatCardSelection;
        var cards = player.Character.CardPool
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            .Where(c => AutomatonCmd.IsEncodable(c) && c.Rarity != CardRarity.Token).ToList();
        var choice = CardFactory.GetDistinctForCombat(player, cards, Amount, rng) .Select(t =>
        {
            t.SetToFreeThisTurn();
            return t;
        });
        await CardPileCmd.Add(choice, PileType.Hand);
    }
}