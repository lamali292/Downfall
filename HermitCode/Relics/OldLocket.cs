using Hermit.HermitCode.Cards.Curse;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Hermit.HermitCode.Relics;

/// <summary>
///     Starter relic. At the start of each combat, add a Memento into your hand.
/// </summary>
public sealed class OldLocket : HermitRelicModel
{
    private bool _firstTurn = true;

    public OldLocket() : base(RelicRarity.Starter)
    {
        WithTips(e => HoverTipFactory.FromCardWithCardHoverTips<MementoCard>());
    }

    public override RelicModel GetUpgradeReplacement()
    {
        return ModelDb.Relic<ClaspedLocket>();
    }


    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!_firstTurn || side != Owner.Creature.Side) return;
        _firstTurn = false;

        Flash();
        var card = combatState.CreateCard<MementoCard>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(
            card,
            PileType.Hand,
            Owner
        );
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        _firstTurn = true;
        return Task.CompletedTask;
    }
}