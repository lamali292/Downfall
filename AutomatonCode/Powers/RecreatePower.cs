using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Powers;

public class RecreatePower : AutomatonPowerModel
{
    public RecreatePower()
    {
        WithTip<Fuel>();
    }


    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (creator == null || creator.Creature != Owner || card.Type != CardType.Status) return;
        var generatedThisTurn = CombatManager.Instance.History.Entries
            .OfType<CardGeneratedEntry>()
            .Count(e => e.HappenedThisTurn(CombatState) && e.Creator == creator  && e.Card.Type == CardType.Status);
        if (generatedThisTurn > Amount) return;
        Flash();
        await CardCmd.TransformTo<Fuel>(card);
    }
}