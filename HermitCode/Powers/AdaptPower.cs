using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Powers;

/// <summary>
///     At the start of your turn, you can Exhaust a card to gain 8 Block.
///     Stacks increase block gained (8 per stack).
/// </summary>
public sealed class AdaptPower : HermitPowerModel
{
    private const int BlockPerStack = 8;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        ICombatState combatState)
    {
        if (side != CombatSide.Player) return;
        if (Owner?.Player == null) return;
        if (CombatManager.Instance.IsOverOrEnding) return;

        var hand = PileType.Hand.GetPile(Owner.Player);
        if (!hand.Cards.Any()) return;

        // Prompt player to select a card to exhaust (optional — min 0, max 1)
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 0, 1);
        var selected = (await CardSelectCmd.FromHand(
            choiceContext,
            Owner.Player,
            prefs,
            null,
            this
        )).FirstOrDefault();

        if (selected != null)
        {
            await CardCmd.Exhaust(choiceContext, selected);
            var blockAmount = BlockPerStack * Amount;
            await CreatureCmd.GainBlock(Owner, blockAmount, default, null);
        }
    }
}