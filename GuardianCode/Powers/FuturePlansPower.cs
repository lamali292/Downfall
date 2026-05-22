using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Powers;

public class FuturePlansPower : GuardianPowerModel
{
    public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext ctx, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != Owner.Side) return;
        var player = Owner.Player;
        if (player == null) return;
        if (GuardianCmd.CanPutIntoStasis(player))
        {
            var cards = await DownfallCardCmd.SelectFromHand(ctx, DownfallCardSelectorPrefs.StasisSelectionPrompt, this, optional: true);
            foreach (var card in cards) await GuardianCmd.PutIntoStasis(card, ctx, this);
        }
    }
}