using Downfall.Code.Abstract;
using Downfall.Code.Cards.Awakened.Token;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Awakened;

public class BloodiedPreeningPower : AwakenedPowerModel
{
    public BloodiedPreeningPower() : base()
    {
        WithTip(typeof(PlumeJab));
    }

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        CombatState combatState)
    {
        if (player.Creature != Owner) return;
        await DownfallCardCmd.GiveCards<PlumeJab>(player, PileType.Hand, Amount, animationTime: 0.1f);
        Flash();
    }
}