using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Powers;

public class LibraryPower : AutomatonPowerModel
{
    protected override async Task AfterSideTurnStart(PlayerChoiceContext ctx, CombatSide side, ICombatState combatState)
    {
        if (side != Owner.Side || Owner.Player == null) return;
        var rng = Owner.CombatState!.RunState.Rng.CombatCardSelection;
        var cards = ModelDb.AllCards
            .Where(c => c is IEncodable { AutoEncode: true })
            .TakeRandom(Amount, rng)
            .Select(t =>
            {
                var card = Owner.CombatState!.CreateCard(t, Owner.Player);
                card.EnergyCost.SetUntilPlayed(0);
                // ? future proof
                card.SetStarCostUntilPlayed(0);
                return card;
            });

        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, Owner.Player);
    }
}