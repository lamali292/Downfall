using Downfall.Code.Abstract;
using Downfall.Code.Cards.Automaton.Token;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Powers.Automaton;

public class InfiniteBeamsUpgradedPower : AutomatonPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Side || Owner.Player == null) return;
        var beams = Enumerable.Range(0, Amount)
            .Select(CardModel (_) =>
            {
                var beam = combatState.CreateCard<MinorBeam>(Owner.Player);
                beam.UpgradeInternal();
                return beam;
            })
            .ToList();

        await CardPileCmd.AddGeneratedCardsToCombat(beams, PileType.Hand, true);
    }
}