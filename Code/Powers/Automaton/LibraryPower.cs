using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Powers.Automaton;

public class LibraryPower : AutomatonPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;


    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
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

        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, true);
    }
}