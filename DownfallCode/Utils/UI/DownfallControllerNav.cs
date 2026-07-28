using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.DownfallCode.Utils.UI;

/// <summary>
///     Wires arbitrary Controls into Godot's focus graph (FocusMode + FocusNeighbor*) for
///     controller navigation, without requiring NClickableControl or a hand-rolled
///     neighbor-linking loop. Mirrors NOrbManager/NCreature's pattern for Defect's orbs,
///     generalized for any character's custom UI.
/// </summary>
public static class DownfallControllerNav
{
    private const string WiredMetaKey = "downfall_controller_nav_wired";

    private const string SelectionReticleScenePath = "res://scenes/ui/selection_reticle.tscn";
    private static readonly StyleBoxEmpty BlankFocusStyle = new();

    // Anchor (creature Hitbox) -> currently-linked group above it. Needed because
    // NCombatRoom.UpdateCreatureNavigation() resets every Hitbox.FocusNeighborTop to a
    // self-loop on every turn boundary, and only Defect auto-repairs that. See
    // CreatureNavigationLinkPatch, which calls ReapplyAnchorLink to restore this for
    // everyone else.
    private static readonly Dictionary<Control, (IReadOnlyList<Control> Controls, int EntryIndex)> AnchorLinks = new();

    /// <summary>
    ///     Sets FocusMode and links FocusNeighborLeft/Right across an ordered list, so
    ///     d-pad/stick left-right moves between them.
    ///     <para />
    ///     Pass <paramref name="wrap" /> for a ring (e.g. a wheel).
    ///     <para />
    ///     <paramref name="rtl" /> for when <paramref name="controls" />' index order doesn't match true
    ///     left-to-right screen position — e.g. because the controls are in a container
    ///     with layout_direction set to RTL. Pass true there so FocusNeighborLeft/Right still point at the correct
    ///     physical neighbor
    ///     <para />
    ///     Safe to call repeatedly on the same list.
    /// </summary>
    public static void WireChain(IReadOnlyList<Control> controls, bool wrap = false, bool rtl = false)
    {
        for (var i = 0; i < controls.Count; i++)
        {
            var control = controls[i];
            control.FocusMode = Control.FocusModeEnum.All;
            control.AddThemeStyleboxOverride("focus", BlankFocusStyle);

            var prev = i > 0 ? controls[i - 1] : wrap && controls.Count > 1 ? controls[^1] : null;
            var next = i < controls.Count - 1 ? controls[i + 1] : wrap && controls.Count > 1 ? controls[0] : null;
            var left = rtl ? next : prev;
            var right = rtl ? prev : next;
            if (left != null) control.FocusNeighborLeft = left.GetPath();
            if (right != null) control.FocusNeighborRight = right.GetPath();
        }
    }

    /// <summary>
    ///     Links a group above <paramref name="anchor" /> (typically a creature's Hitbox): "up"
    ///     from the anchor enters the group at <paramref name="entryIndex" />, "down" from the
    ///     group returns to the anchor. Matches NOrbManager/NCreature's Top/Bottom convention.
    ///     <paramref name="entryIndex" />  is what controls which control is reached first
    /// </summary>
    public static void LinkAbove(IReadOnlyList<Control> controls, Control anchor, int entryIndex = 0)
    {
        PruneDeadAnchors();

        if (controls.Count == 0)
        {
            AnchorLinks.Remove(anchor);
            return;
        }

        AnchorLinks[anchor] = (controls, entryIndex);
        ApplyAnchorLink(anchor);
    }

    // Called by CreatureNavigationLinkPatch after every base-game navigation refresh, not
    // meant to be called directly by widget code (LinkAbove already applies immediately).
    public static void ReapplyAnchorLink(Control anchor)
    {
        if (!GodotObject.IsInstanceValid(anchor))
        {
            AnchorLinks.Remove(anchor);
            return;
        }

        ApplyAnchorLink(anchor);
    }

    // Drops anchors whose Hitbox has been freed. Without this, a creature that dies (or a
    // room that ends) without a follow-up LinkAbove/ReapplyAnchorLink for its anchor leaves
    // a stale key here forever — a static reference pinning the managed wrapper for the rest
    // of the process. Same static-state-outliving-its-objects trap as the old _reticleScene
    // cache; over a long multiplayer session these accumulate.
    private static void PruneDeadAnchors()
    {
        List<Control>? dead = null;
        foreach (var anchor in AnchorLinks.Keys)
        {
            if (GodotObject.IsInstanceValid(anchor)) continue;
            (dead ??= new List<Control>()).Add(anchor);
        }

        if (dead == null) return;
        foreach (var anchor in dead) AnchorLinks.Remove(anchor);
    }

    private static void ApplyAnchorLink(Control anchor)
    {
        if (!AnchorLinks.TryGetValue(anchor, out var link)) return;

        // The anchor itself can be freed (creature died) while this is called from a
        // deferred/patch path — bail rather than throwing on GetPath() below.
        if (!GodotObject.IsInstanceValid(anchor))
        {
            AnchorLinks.Remove(anchor);
            return;
        }

        // A linked group can be freed out from under this
        // (like Champ's icons when the stance ends)
        // while the anchor itself (the creature's Hitbox) stays alive.
        // Validate and drop any stale entries so we don't throw an error downstream
        foreach (var control in link.Controls)
        {
            if (GodotObject.IsInstanceValid(control)) continue;
            AnchorLinks.Remove(anchor);
            return;
        }

        anchor.FocusNeighborTop = link.Controls[link.EntryIndex].GetPath();
        foreach (var control in link.Controls)
            control.FocusNeighborBottom = anchor.GetPath();
    }

    /// <summary>
    ///     Unifies mouse-hover and controller-focus on a single Control into one
    ///     onFocus/onUnfocus pair, without subclassing NClickableControl. Idempotent per
    ///     instance via node metadata, so it's safe to call again on a pooled/reused Control.
    /// </summary>
    public static void WireHover(Control control, Action onFocus, Action onUnfocus)
    {
        control.FocusMode = Control.FocusModeEnum.All;
        control.AddThemeStyleboxOverride("focus", BlankFocusStyle);

        if (control.HasMeta(WiredMetaKey)) return;
        control.SetMeta(WiredMetaKey, true);

        var isHovered = false;
        var isFocused = false;
        var wasActive = false;

        void Refresh()
        {
            var active = isHovered || isFocused;
            if (active == wasActive) return;
            wasActive = active;
            if (active) onFocus();
            else onUnfocus();
        }

        control.Connect(Control.SignalName.MouseEntered, Callable.From(() =>
        {
            isHovered = true;
            Refresh();
        }));
        control.Connect(Control.SignalName.MouseExited, Callable.From(() =>
        {
            isHovered = false;
            Refresh();
        }));
        control.Connect(Control.SignalName.FocusEntered, Callable.From(() =>
        {
            isFocused = true;
            Refresh();
        }));
        control.Connect(Control.SignalName.FocusExited, Callable.From(() =>
        {
            isFocused = false;
            Refresh();
        }));
    }

    /// <summary>
    ///     Instantiates the base game's own focus reticle (res://scenes/ui/selection_reticle.tscn,
    ///     the bracket Defect's orbs use) sized/positioned around an arbitrary hitbox. Caller
    ///     drives visibility via the returned reticle's OnSelect()/OnDeselect().
    ///     <para />
    ///     Loads per call rather than caching a static PackedScene: the reticle scene isn't in
    ///     any PreloadManager asset set, so it gets unloaded on room transitions and a cached
    ///     wrapper would be disposed out from under us (the original ObjectDisposedException).
    ///     ResourceLoader.Load reuses the game's cached copy when it's still loaded, reloads it
    ///     when it isn't.
    /// </summary>
    public static NSelectionReticle? AttachFocusReticle(Node parent, Vector2 center, Vector2 hitboxSize,
        float margin = 12f)
    {
        var scene = ResourceLoader.Load<PackedScene>(SelectionReticleScenePath);
        if (scene == null)
        {
            GD.PushWarning($"[DownfallControllerNav] Failed to load reticle scene: {SelectionReticleScenePath}");
            return null;
        }

        var reticle = scene.Instantiate<NSelectionReticle>();
        var half = hitboxSize / 2f + new Vector2(margin, margin);
        reticle.Position = center - half;
        reticle.Size = half * 2f;
        parent.AddChild(reticle);
        return reticle;
    }

    /// <summary>WireChain + WireHover across a row in one call, with per-index callbacks.</summary>
    public static void WireHoverChain(
        IReadOnlyList<Control> controls,
        Action<int> onFocus,
        Action<int> onUnfocus,
        bool wrap = false,
        bool rtl = false)
    {
        WireChain(controls, wrap, rtl);
        for (var i = 0; i < controls.Count; i++)
        {
            var index = i;
            WireHover(controls[i], () => onFocus(index), () => onUnfocus(index));
        }
    }
}