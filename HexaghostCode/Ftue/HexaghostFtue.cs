using Downfall.DownfallCode.Utils.UI.Ftue;
using Godot;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Hexaghost.HexaghostCode.Ftue;

public static class HexaghostFtue
{
    public static readonly DownfallFtue.Tip Wheel = new("hexaghost_wheel_ftue", "HEXAGHOST-WHEEL_FTUE");

    private static  Vector2 ArrowFromTarget => new(30, -30);
    private static Vector2 PopupFromArrow => new(275, 180);

    public static void QueueWheel(Player player) =>
        DownfallFtue.QueuePointer(Wheel, player, FindTarget, ArrowFromTarget, PopupFromArrow, 165);

    private static DownfallFtue.PointerTarget? FindTarget(Player player)
    {
        var visuals = HexaghostVisualsBridge.GetVisuals(player);
        if (visuals == null || !GodotObject.IsInstanceValid(visuals)) return null;
        var index = HexaghostCmd.GetCurrentIndex(player);
        return new DownfallFtue.PointerTarget(visuals.GetFlameWorldPosition(index), visuals);
    }
}
