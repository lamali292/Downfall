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
    public readonly record struct PointerItem(
        Tip Tip, Func<Player, PointerTarget?> FindTarget, Vector2 ArrowFromTarget, Vector2 PopupFromArrow,
        float? ArrowPointDirectionDegrees = null);

    public enum ComboMode
    {
        Sequential, // one tip at a time; dismissing one ("got it") shows the next
        Simultaneous, // all tips shown together; each dismisses independently
    }

    private const string LocTable = "ftues";
    private const string PointerScene = "res://Downfall/scenes/ftue/pointer_ftue.tscn";
    private const string RulesScene = "res://Downfall/scenes/ftue/rules_ftue.tscn";
    private const string ComboScene = "res://Downfall/scenes/ftue/combo_ftue.tscn";

    private const double TickSeconds = 0.25;
    private const int MaxTicks = 600;
    
    private const int SettleTicks = 4;
    
    private const bool ForceShow = false;

    private static readonly HashSet<string> Pending = new();

    public static void QueueRules(Tip tip, Player player, params string?[] imagePaths) =>
        Queue(tip, player, (modal, _) =>
        {
            ShowRules(tip, imagePaths, popup => modal.Add(popup));
            return true;
        });

    public static void QueuePointer(Tip tip, Player player,
        Func<Player, PointerTarget?> findTarget, Vector2 arrowFromTarget, Vector2 popupFromArrow,
        float? arrowPointDirectionDegrees = null) =>
        Queue(tip, player, (modal, p) =>
        {
            if (findTarget(p) is not { } target) return false;
            ShowPointer(tip, popup => modal.Add(popup), p, findTarget, target, arrowFromTarget, popupFromArrow,
                arrowPointDirectionDegrees: arrowPointDirectionDegrees);
            return true;
        });

    // Shows several pointer tips together on the same screen (one shared backstop, no re-wait between
    // them), either one at a time (`Sequential`, default) or all at once (`Simultaneous`). Either way
    // the screen only fully closes once every tip shown has been dismissed. Items already marked as
    // seen are left out, so a single still-unseen item just shows normally.
    public static void QueueComboPointer(Player player, ComboMode mode, params PointerItem[] items)
    {
        if (TestMode.IsOn) return;
        var comboId = string.Join("+", items.Select(i => i.Tip.Id));
        if (!Pending.Add(comboId)) return;
        TaskHelper.RunSafely(ShowComboWhenClear(comboId, player, items, mode));
    }

    // Defaults to Sequential — pass a mode explicitly to show every tip at once instead.
    public static void QueueComboPointer(Player player, params PointerItem[] items) =>
        QueueComboPointer(player, ComboMode.Sequential, items);

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

    private static void ShowRules(Tip tip, string?[] imagePaths, Action<Control> attach, Action? onDismissed = null)
    {
        var popup = ResourceLoader.Load<PackedScene>(RulesScene).Instantiate<NDownfallRulesFtue>();
        popup.OnDismissed = onDismissed;
        attach(popup);
        var pageCount = imagePaths.Length;
        var pages = new string[pageCount];
        var images = new Texture2D?[pageCount];
        for (var i = 0; i < pageCount; i++)
        {
            pages[i] = new LocString(LocTable, $"{tip.LocKey}.body{i + 1}").GetFormattedText();
            images[i] = imagePaths[i] is { } path ? ResourceLoader.Load<Texture2D>(path) : null;
        }
        popup.SetText(new LocString(LocTable, tip.LocKey + ".title").GetFormattedText(), pages, images);
    }

    private static void ShowPointer(Tip tip, Action<Control> attach, Player player,
        Func<Player, PointerTarget?> findTarget, PointerTarget target,
        Vector2 arrowFromTarget, Vector2 popupFromArrow, Action? onDismissed = null,
        float? arrowPointDirectionDegrees = null)
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

        popup.OnDismissed = onDismissed;
        attach(popup);
        popup.SetText(
            new LocString(LocTable, tip.LocKey + ".title").GetFormattedText(),
            new LocString(LocTable, tip.LocKey + ".description").GetFormattedText());
        // Track (not a one-shot PointAt) so the arrow keeps following its target across a window rescale.
        popup.Track(() => findTarget(player), arrowFromTarget, popupFromArrow, arrowPointDirectionDegrees);
    }

    private static async Task ShowComboWhenClear(string comboId, Player player, PointerItem[] items, ComboMode mode)
    {
        try
        {
            var settled = 0;
            for (var tick = 0; tick < MaxTicks; tick++)
            {
                await Wait(TickSeconds);
                if (!CombatManager.Instance.IsInProgress || !player.Creature.IsAlive) return;

                var toShow = ForceShow
                    ? items
                    : items.Where(i => SaveManager.Instance is not { } save || !save.SeenFtue(i.Tip.Id)).ToArray();
                if (toShow.Length == 0) return;

                if (NModalContainer.Instance is not { OpenModal: null } modal
                    || NCombatRoom.Instance is not { } room
                    || room.GetChildren().OfType<NCombatStartBanner>().Any())
                {
                    settled = 0;
                    continue;
                }

                var targets = new PointerTarget[toShow.Length];
                var allReady = true;
                for (var i = 0; i < toShow.Length; i++)
                {
                    if (toShow[i].FindTarget(player) is not { } target)
                    {
                        allReady = false;
                        break;
                    }
                    targets[i] = target;
                }
                if (!allReady)
                {
                    settled = 0;
                    continue;
                }
                if (++settled < SettleTicks) continue;

                ShowCombo(toShow, targets, player, modal, mode);
                return;
            }
        }
        finally
        {
            Pending.Remove(comboId);
        }
    }

    private static void ShowCombo(PointerItem[] items, PointerTarget[] targets, Player player,
        NModalContainer modal, ComboMode mode)
    {
        var host = ResourceLoader.Load<PackedScene>(ComboScene).Instantiate<NDownfallComboFtue>();
        modal.Add(host);
        if (mode == ComboMode.Simultaneous)
            ShowComboSimultaneous(items, targets, player, host);
        else
            ShowComboStep(items, targets, player, host, 0);
    }

    // All items shown together; the host closes once every one of them has been dismissed.
    private static void ShowComboSimultaneous(PointerItem[] items, PointerTarget[] targets, Player player,
        NDownfallComboFtue host)
    {
        host.PrepareSimultaneous(items.Length);
        for (var i = 0; i < items.Length; i++)
        {
            var tip = items[i].Tip;
            var findTarget = items[i].FindTarget;
            ShowPointer(tip, popup => host.AddChildSafely(popup), player, findTarget, targets[i],
                items[i].ArrowFromTarget, items[i].PopupFromArrow, onDismissed: () =>
                {
                    SaveManager.Instance.MarkFtueAsComplete(tip.Id);
                    host.ReleaseSimultaneous();
                },
                arrowPointDirectionDegrees: items[i].ArrowPointDirectionDegrees);
        }
    }

    // Shows items[index], then on dismissal recurses into index+1; once past the last item, closes
    // the shared host instead of showing anything more.
    private static void ShowComboStep(PointerItem[] items, PointerTarget[] targets, Player player,
        NDownfallComboFtue host, int index)
    {
        if (index >= items.Length)
        {
            host.Finish();
            return;
        }

        var tip = items[index].Tip;
        var findTarget = items[index].FindTarget;
        // Re-resolve rather than trusting the stale pre-fetched target: earlier items may have taken a
        // while to dismiss, so this item's target could have moved (or, defensively, disappeared).
        var target = findTarget(player) ?? targets[index];
        ShowPointer(tip, popup => host.AddChildSafely(popup), player, findTarget, target,
            items[index].ArrowFromTarget, items[index].PopupFromArrow, onDismissed: () =>
            {
                SaveManager.Instance.MarkFtueAsComplete(tip.Id);
                ShowComboStep(items, targets, player, host, index + 1);
            },
            arrowPointDirectionDegrees: items[index].ArrowPointDirectionDegrees);
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
