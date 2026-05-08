using Downfall.DownfallCode.Abstract;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.DownfallCode.Powers;

public class StunnedPower() : DownfallPowerModel(PowerType.Debuff, PowerStackType.Single)
{
    public bool Active;

    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side) return Task.CompletedTask;
        Active = true;
        return Task.CompletedTask;
    }

    protected override async Task AfterSideTurnStart(PlayerChoiceContext ctx, CombatSide side, ICombatState combatState)
    {
        if (side != Owner.Side) return;
        await PowerCmd.Remove(this);
    }
}

[HarmonyPatch(typeof(CombatManager))]
public static class StunnedMultiplayerPatch
{
    [HarmonyPatch(nameof(CombatManager.PlayerActionsDisabled), MethodType.Getter)]
    [HarmonyPostfix]
    private static void Postfix(ref bool __result, CombatManager __instance)
    {
        var me = LocalContext.GetMe(__instance.DebugOnlyGetState());
        var power = me?.Creature.GetPower<StunnedPower>();
        if (me != null && power != null) __result = power.Active;
    }

    [HarmonyPatch(nameof(CombatManager.SetupPlayerTurn))]
    [HarmonyPrefix]
    private static bool PrefixSetup(Player player, ref Task __result, CombatManager __instance)
    {
        if (!player.Creature.HasPower<StunnedPower>()) return true;
        __instance.SetReadyToEndTurn(player, false);
        __result = Task.CompletedTask;
        return false;
    }


    [HarmonyPatch(nameof(CombatManager.RunAutoPrePlayPhase))]
    [HarmonyPrefix]
    private static bool PrefixPrePlay(Player player, ref Task __result)
    {
        if (!player.Creature.HasPower<StunnedPower>()) return true;
        __result = Task.CompletedTask;
        return false;
    }
}