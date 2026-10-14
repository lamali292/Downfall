using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Powers;

public class InfiniteBeamsPower : AutomatonPowerModel
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player != Owner.Player) return;
        await DownfallCardCmd.GiveCards<MinorBeam>(player, PileType.Hand, Amount);
    }
}