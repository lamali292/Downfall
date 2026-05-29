using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Powers;

public class RecreatePower : AutomatonPowerModel
{
    public RecreatePower()
    {
        this.WithTip<Fuel>();
    }


    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (creator == null || creator.Creature != Owner) return;
        var generatedThisTurn = CombatManager.Instance.History.Entries
            .OfType<CardGeneratedEntry>()
            .Count(e => e.HappenedThisTurn(CombatState) && e.Creator == creator);
        if (generatedThisTurn > Amount) return;
        Flash();
        await CardCmd.TransformTo<Fuel>(card);
    }
}