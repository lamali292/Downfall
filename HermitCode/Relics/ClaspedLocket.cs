using Downfall.DownfallCode.Compatibility;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Hermit.HermitCode.Relics;

/// <summary>
///     Replaces [gold]Old Locket[/gold]. First time you draw a curse each turn, [gold]Exhaust[/gold] it and draw 2 cards.
///     Upon pickup, obtain 2 [gold]Injuries[/gold].
/// </summary>
[Obsolete]
public sealed class ClaspedLocket : HermitRelicModel
{
    private bool _usedThisTurn;

    public ClaspedLocket() : base(RelicRarity.Starter, false)
    {
        WithVars(new CardsVar(3));
        WithVar("Curses", 2);
        WithTip<Injury>();
    }

    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        for (var i = 0; i < DynamicVars["Curses"].BaseValue; i++)
        {
            CardModel card = Owner.RunState.CreateCard<Injury>(Owner);
            var result = await CardPileCmd.Add(card, PileType.Deck);
            CardCmd.PreviewCardPileAdd([result], 2f);
        }
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext ctx, CardModel card, bool fromHandDraw)
    {
        if (card.Owner == Owner && card.Type == CardType.Curse && !_usedThisTurn)
        {
            _usedThisTurn = true;
            Status = RelicStatus.Normal;
            Flash();
            await CardCmdCompatibility.Exhaust(ctx, card);
            await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, Owner);
        }
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature)) return Task.CompletedTask;
        _usedThisTurn = false;
        Status = RelicStatus.Active;
        return Task.CompletedTask;
    }
}