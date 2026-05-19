using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Powers;

/// <summary>
///     At the start of your turn, you can Exhaust a card to gain 8 Block.
///     Stacks increase block gained (8 per stack).
/// </summary>
public sealed class AdaptPower : HermitPowerModel
{
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        ICombatState combatState)
    {
        if (side != CombatSide.Player) return;
        if (Owner.Player == null) return;
        if (CombatManager.Instance.IsOverOrEnding) return;
        var hand = PileType.Hand.GetPile(Owner.Player);
        if (!hand.Cards.Any()) return;
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 0, Amount);
        var selected = await CardSelectCmd.FromHand(
            choiceContext,
            Owner.Player,
            prefs,
            null,
            this
        );
        foreach (var card in selected)
        {
            await CardCmd.Exhaust(choiceContext, card);
            await CreatureCmd.GainBlock(Owner, 8, ValueProp.Unpowered, null);
        }
        
    }
}