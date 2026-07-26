using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Powers;

public class RecreatePower : AutomatonPowerModel
{
    public RecreatePower()
    {
        WithTip<Fuel>();
    }


    public override int DisplayAmount => Math.Max(Amount - GeneratedThisTurn, 0);

    private int GeneratedThisTurn => CombatManager.Instance.History.Entries
        .OfType<CardGeneratedEntry>()
        .Count(e => e.HappenedThisTurn(CombatState) && e.Creator?.Creature == Owner && e.Card.Type == CardType.Status);

    public override Task AfterSideTurnStart(CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return Task.CompletedTask;
        this.InvokeSilentDisplayAmountChanged();
        return Task.CompletedTask;
    }


    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (creator == null || creator.Creature != Owner || card.Type != CardType.Status) return;
        this.InvokeSilentDisplayAmountChanged();
        if (GeneratedThisTurn > Amount) return;
        Flash();
        await CardCmd.TransformTo<Fuel>(card);
    }
}