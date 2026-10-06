using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;

namespace Downfall.DownfallCode.Utils.UI.Ftue;

public static class DownfallFtue
{
    public readonly record struct Tip(string Id, string LocKey);
    public readonly record struct PointerTarget(Vector2 Aim, CanvasItem? ZBoost);

    private const string LocTable = "ftues";
    private const string PointerScene = "res://Downfall/scenes/ftue/pointer_ftue.tscn";

    private const double TickSeconds = 0.25;
    private const int MaxTicks = 600;
    
    private const int SettleTicks = 4;
    
    private const bool ForceShow = true;

    private static readonly HashSet<string> Pending = new();

    public static void QueueRules(Tip tip, Player player, string scenePath, int pageCount) =>
        Queue(tip, player, (modal, _) =>
        {
            ShowRules(tip, scenePath, pageCount, modal);
            return true;
        });

    public static void QueuePointer(Tip tip, Player player,
        Func<Player, PointerTarget?> findTarget, Vector2 arrowFromTarget, Vector2 popupFromArrow) =>
        Queue(tip, player, (modal, p) =>
        {
            if (findTarget(p) is not { } target) return false;
            ShowPointer(tip, modal, target, arrowFromTarget, popupFromArrow);
            return true;
        });

    private static void Queue(Tip tip, Player player, Func<NModalContainer, Player, bool> show)
    {
        if (TestMode.IsOn) return;
        if (!ForceShow && (SaveManager.Instance is not { } save || save.SeenFtue(tip.Id))) return;
        if (!Pending.Add(tip.Id)) return;
        TaskHelper.RunSafely(ShowWhenClear(tip, player, show));
    }
    
    private static async Task ShowWhenClear(Tip tip, Player player, Func<NModalContainer, Player, bool> show)
    {
        try
        {
            var settled = 0;
            for (var tick = 0; tick < MaxTicks; tick++)
            {
                await Wait(TickSeconds);
                if (!CombatManager.Instance.IsInProgress || !player.Creature.IsAlive) return;
                if (!ForceShow && SaveManager.Instance.SeenFtue(tip.Id)) return;
                if (NModalContainer.Instance is not { OpenModal: null } modal
                    || NCombatRoom.Instance is not { } room
                    || room.GetChildren().OfType<NCombatStartBanner>().Any())
                {
                    settled = 0;
                    continue;
                }
                if (++settled < SettleTicks) continue;
                if (!show(modal, player)) return;
                SaveManager.Instance.MarkFtueAsComplete(tip.Id);
                return;
            }
        }
        finally
        {
            Pending.Remove(tip.Id);
        }
    }

    // Cmd.Wait returns at once in Instant fast mode, which would turn the poll into a busy loop
    private static async Task Wait(double seconds)
    {
        var timer = ((SceneTree)Engine.GetMainLoop()).CreateTimer(seconds);
        await timer.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
    }

    private static void ShowRules(Tip tip, string scenePath, int pageCount, NModalContainer modal)
    {
        var popup = ResourceLoader.Load<PackedScene>(scenePath).Instantiate<NDownfallRulesFtue>();
        modal.Add(popup);
        var pages = new string[pageCount];
        for (var i = 0; i < pageCount; i++)
            pages[i] = new LocString(LocTable, $"{tip.LocKey}.body{i + 1}").GetFormattedText();
        popup.SetText(new LocString(LocTable, tip.LocKey + ".title").GetFormattedText(), pages);
    }

    private static void ShowPointer(Tip tip, NModalContainer modal, PointerTarget target,
        Vector2 arrowFromTarget, Vector2 popupFromArrow)
    {
        var popup = ResourceLoader.Load<PackedScene>(PointerScene).Instantiate<NDownfallPointerFtue>();

        // Like the base tips, the thing pointed at draws above the backstop for as long as the tip is up
        if (target.ZBoost is { } boost)
        {
            var defaultZ = boost.ZIndex;
            boost.ZIndex = defaultZ + 1 - EffectiveZ(boost);
            popup.Connect(Node.SignalName.TreeExited, Callable.From(() =>
            {
                if (GodotObject.IsInstanceValid(boost)) boost.ZIndex = defaultZ;
            }));
        }

        modal.Add(popup);
        popup.SetText(
            new LocString(LocTable, tip.LocKey + ".title").GetFormattedText(),
            new LocString(LocTable, tip.LocKey + ".description").GetFormattedText());
        popup.PointAt(target.Aim, arrowFromTarget, popupFromArrow);
    }

    // The z the backstop competes with: every ancestor's z_index adds up while z_as_relative holds
    private static int EffectiveZ(CanvasItem item)
    {
        var z = 0;
        for (Node? node = item; node is CanvasItem ci; node = node.GetParent())
        {
            z += ci.ZIndex;
            if (!ci.ZAsRelative) break;
        }
        return z;
    }
}
