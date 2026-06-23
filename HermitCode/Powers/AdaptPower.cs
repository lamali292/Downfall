using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Powers;

public sealed class AdaptPower : HermitPowerModel
{
    protected override async Task AfterSideTurnStart(PlayerChoiceContext ctx, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player) return;
        if (Owner.Player == null) return;
        if (CombatManager.Instance.IsOverOrEnding) return;
        var hand = PileType.Hand.GetPile(Owner.Player);
        if (!hand.Cards.Any()) return;
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 0, Amount);
        var selected = await CardSelectCmd.FromHand(
            ctx,
            Owner.Player,
            prefs,
            null,
            this
        );
        foreach (var card in selected)
        {
            await CardCmd.Exhaust(ctx, card);
            await CreatureCmd.GainBlock(Owner, 8, ValueProp.Unpowered, null);
        }
    }
}