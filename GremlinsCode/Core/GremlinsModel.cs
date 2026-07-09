using BaseLib.Abstracts;
using Gremlins.GremlinsCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Gremlins.GremlinsCode.Core;

public class GremlinsModel() : CustomSingletonModel(HookType.Combat)
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player.Character is not Gremlins || player.PlayerCombatState is not { TurnNumber: 1 }) return;
        await PowerCmd.Apply<GremlinPower>(ctx, player.Creature, 1, player.Creature, null, true);
    }

    public static Task? OnDeath(Creature creature)
    {
        var player = creature.Player;
        if (player?.Character is not Gremlins) return null;

        var state = GremlinsRunModel.GetState(player);
        var dying = state.Active;
        if (dying == null || !state.Bench.Any()) return null;

        return SwapGremlinAsync(player, dying);
    }
    
    private static async Task SwapGremlinAsync(Player player, Creature dying)
    {
        if (player.Creature.CombatState == null || dying.Monster == null) return;
        var hookCtx = new HookPlayerChoiceContext(dying.Monster, player.NetId,
            player.Creature.CombatState, GameActionType.Combat);
        await Cmd.Wait(0.5f);
        await GremlinsCmd.KillGremlin(hookCtx, player, dying);
        dying.Monster.InvokeExecutionFinished();
    }
}
