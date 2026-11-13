using Awakened.AwakenedCode.Displays;
using Awakened.AwakenedCode.Vfx;
using Downfall.DownfallCode.Utils.UI;
using Downfall.DownfallCode.Utils.UI.Ftue;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Awakened.AwakenedCode.Ftue;

public static class AwakenedFtue
{
    public static readonly DownfallFtue.Tip Spellbook = new("awakened_spellbook_ftue", "AWAKENED-SPELLBOOK_FTUE");
    public static readonly DownfallFtue.Tip AwakenMeter = new("awakened_awaken_meter_ftue", "AWAKENED-AWAKEN_FTUE");

    private static Vector2 SpellbookArrowFromTarget => new(70, -10);
    private static Vector2 SpellbookPopupFromArrow => new(270, 200);
    
    private static Vector2 MeterArrowFromTarget => new(30, -100);
    private static Vector2 MeterPopupFromArrow => new(200, 0);
    
    public static void QueueSpellbookAndMeter(Player player) =>
        DownfallFtue.QueueComboPointer(player,
            new DownfallFtue.PointerItem(Spellbook, FindSpellbookTarget, SpellbookArrowFromTarget, SpellbookPopupFromArrow, 170),
            new DownfallFtue.PointerItem(AwakenMeter, FindMeterTarget, MeterArrowFromTarget, MeterPopupFromArrow,130));

    private static DownfallFtue.PointerTarget? FindSpellbookTarget(Player player)
    {
        var btn = NCustomCombatCardPile.GetPileNode<NSpellbookButton>();
        if (btn == null || !GodotObject.IsInstanceValid(btn)) return null;
        return new DownfallFtue.PointerTarget(NCustomCombatCardPile.GetPositionFor<NSpellbookButton>(), btn);
    }

    private static DownfallFtue.PointerTarget? FindMeterTarget(Player player)
    {
        var meter = AwakenedDisplay.GetAwakenMeter(player);
        if (meter == null || !GodotObject.IsInstanceValid(meter)) return null;
        var aim = meter.GetGlobalTransform() * new Vector2(meter.Size.X / 2f, meter.Size.Y);
        return new DownfallFtue.PointerTarget(aim, meter);
    }
}
