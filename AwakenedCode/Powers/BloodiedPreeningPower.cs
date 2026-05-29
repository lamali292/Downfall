using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Powers;

public class BloodiedPreeningPower : AwakenedPowerModel
{
    public BloodiedPreeningPower()
    {
        this.WithTip<PlumeJab>();
    }

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext ctx,
        ICombatState combatState)
    {
        if (player.Creature != Owner) return;
        await DownfallCardCmd.GiveCards<PlumeJab>(player, PileType.Hand, Amount, animationTime: 0.1f);
        Flash();
    }
}