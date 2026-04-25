using Downfall.Code.Abstract;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Guardian;

public class FuturePlansPower : GuardianPowerModel
{
    public override async Task BeforeTurnEndEarly(PlayerChoiceContext ctx, CombatSide side)
    {
        if (side != Owner.Side) return;
        var player = Owner.Player;
        if (player == null) return;
        if (GuardianCmd.CanPutIntoStasis(player))
        {
            var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, Amount);
            var cards = await CardSelectCmd.FromHand(ctx, player, prefs, null, this);
            foreach (var card in cards) await GuardianCmd.PutIntoStasis(card, ctx, this);
        }
    }
}