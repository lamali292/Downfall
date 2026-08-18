using Downfall.DownfallCode.Powers;

using Gremlins.GremlinsCode.Core;
using Gremlins.GremlinsCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Gremlins.GremlinsCode.Powers;

public class NobPower()
    : GremlinsPowerModel(PowerType.Buff, PowerStackType.Single), IShouldGremlinSwap, IAfterGremlinSwap
{
    private static readonly LocString GremlinNobDialogue = new("monsters", "GREMLINS-GREMLIN_NOB.banter");
    
    public async Task AfterGremlinSwap(PlayerChoiceContext ctx, Player player, GremlinSwapType gremlinSwapType)
    {
        if (gremlinSwapType != GremlinSwapType.Death) return;
        await PowerCmd.Remove(this);
    }

    public bool ShouldGremlinSwap(Player player, Creature gremlin)
    {
        return player.Creature != Owner || gremlin.Monster is GremlinNob;
    }

    protected override async Task AfterApplied(PlayerChoiceContext ctx, Creature? applier, CardModel? cardSource)
    {
        var player = Owner.Player;
        if (player == null) return;
        GremlinsCmd.AddGremlin(player, ModelDb.Monster<GremlinNob>(), 20, 20);
        await GremlinsCmd.SwapToType<GremlinNob>(ctx, player);
        await Cmd.Wait(1);
        var a = GremlinsCmd.GetCurrentGremlin(player);
        if (a == null) return;
        TalkCmd.Play(GremlinNobDialogue, a, VfxColor.Red);
        SfxCmd.Play("event:/sfx/characters/gremlins-gremlins/nob");
    }


    public override async Task AfterPowerAmountChanged(PlayerChoiceContext ctx, PowerModel power, decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power is TempHpPower && power.Amount <= 0) await PowerCmd.Remove(this);
    }


    protected override async Task AfterRemoved(PlayerChoiceContext ctx, Creature oldOwner)
    {
        if (Owner.Player == null) return;
        var a = GremlinsCmd.GetCurrentGremlin(Owner.Player);
        if (a is not { Monster: GremlinNob }) return;
        await GremlinsCmd.KillGremlin(ctx, Owner.Player, a);
    }
}