using Downfall.Code.Abstract;
using Downfall.Code.Cards.Awakened.Token;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Awakened;

public class ThaumaturgyPower : AwakenedPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        CombatState combatState)
    {
        if (player.Creature != Owner) return;
        await DownfallCardCmd.GiveCard<Ceremony>(player, PileType.Hand, animationTime: 0.1f);
        await PowerCmd.Decrement(this);
    }
}