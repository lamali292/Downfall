using System.Reflection;
using BaseLib.Abstracts;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Downfall.Code.Powers.Abstract;

public abstract class PowerNextTurn<T> : CustomPowerModel, IColoredPower
    where T : PowerModel
{
    public override string CustomPackedIconPath => ModelDb.Power<T>().PackedIconPath;
    public override string CustomBigIconPath => ModelDb.Power<T>().ResolvedBigIconPath;
    public override PowerType Type => ModelDb.Power<T>().Type;
    public override PowerStackType StackType => ModelDb.Power<T>().StackType;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => ModelDb.Power<T>().HoverTips;

    public override List<(string, string)> Localization => new PowerLoc(
        $"Next Turn {ModelDb.Power<T>().Title.GetFormattedText()}",
        $"Next turn, gain [gold]{ModelDb.Power<T>().Title.GetFormattedText()}[/gold].",
        $"Next turn, gain {{Amount:plural:|{{Amount}} }}[gold]{ModelDb.Power<T>().Title.GetFormattedText()}[/gold]."
    );

    public virtual Color IconColor => Colors.Green;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player.Creature != Owner) return;
        await PowerCmd.Remove(this);
        await PowerCmd.Apply<T>(Owner, Amount, Applier, null);
    }
}

[HarmonyPatch(typeof(NPower), "Reload")]
internal static class NPowerReloadPatch
{
    private static void Postfix(NPower __instance)
    {
        if (__instance.Model is IColoredPower colored)
            __instance.GetNode<TextureRect>("%Icon").Modulate = colored.IconColor;
    }
}

[HarmonyPatch(typeof(NPowerAppliedVfx), "Create")]
internal static class NPowerAppliedVfxCreatePatch
{
    private static void Postfix(NPowerAppliedVfx? __result, PowerModel power)
    {
        if (__result == null) return;
        if (power is IColoredPower colored)
            __result.Modulate = colored.IconColor;
    }
}

[HarmonyPatch(typeof(NPowerFlashVfx), "Create")]
internal static class NPowerFlashVfxCreatePatch
{
    private static void Postfix(NPowerFlashVfx? __result, PowerModel power)
    {
        if (__result == null) return;
        if (power is IColoredPower colored)
            __result.Modulate = colored.IconColor;
    }
}

[HarmonyPatch(typeof(NPowerRemovedVfx), "Create")]
internal static class NPowerRemovedVfxCreatePatch
{
    private static void Postfix(NPowerRemovedVfx? __result, PowerModel power)
    {
        if (__result == null) return;
        if (power is IColoredPower colored)
            __result.Modulate = colored.IconColor;
    }
}

[HarmonyPatch]
internal static class NHoverTipSetInitPatch
{
    private static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(NHoverTipSet), "Init");
    }

    private static void Postfix(NHoverTipSet __instance, IEnumerable<IHoverTip> hoverTips)
    {
        var tips = hoverTips.ToList();
        Callable.From(() =>
        {
            if (!GodotObject.IsInstanceValid(__instance)) return;
            var container = (Control)AccessTools.Field(typeof(NHoverTipSet), "_textHoverTipContainer")
                .GetValue(__instance)!;

            var tipList = tips.OfType<HoverTip>().ToList();
            var children = container.GetChildren().OfType<Control>().ToList();

            for (var i = 0; i < Math.Min(tipList.Count, children.Count); i++)
            {
                var ht = tipList[i];
                try
                {
                    var modelId = ModelId.Deserialize(ht.Id);
                    var power = ModelDb.GetById<PowerModel>(modelId);
                    if (power is not IColoredPower colored) continue;

                    var iconRect = children[i].GetNodeOrNull<TextureRect>("%Icon");
                    if (iconRect != null)
                        iconRect.Modulate = colored.IconColor;
                }
                catch
                {
                    /* not a power or not found */
                }
            }
        }).CallDeferred();
    }
}

public interface IColoredPower
{
    Color IconColor { get; }
}