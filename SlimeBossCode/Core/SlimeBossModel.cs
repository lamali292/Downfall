using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SlimeBoss.SlimeBossCode.Core;

public class SlimeBossModel() : CustomSingletonModel(HookType.Combat)
{
    
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext ctx, CombatSide side, IEnumerable<Creature> participants)
    {
        foreach (var player in participants.Where(e => e.IsPlayer).Select(e => e.Player).OfType<Player>())
        {
            await SlimeBossCmd.AutomaticCommandAll(ctx, player);
        }
    }
    
}