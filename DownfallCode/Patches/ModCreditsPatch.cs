namespace Downfall.DownfallCode.Patches;

using System;
using System.Collections.Generic;
using BaseLib.Extensions;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Screens.Credits;

/// <summary>
/// Public registry for adding mod credit sections to the vanilla credits screen.
/// Mods call one of the <c>Register</c> overloads at load time; the sections are
/// rendered later by <see cref="ModCreditsPatch"/> when the screen opens.
/// </summary>
/// <remarks>
/// All text is resolved from the vanilla <c>credits</c> loc table, namespaced by
/// mod id: <c>&lt;MODID&gt;-&lt;SECTION&gt;.header</c> and
/// <c>&lt;MODID&gt;-&lt;SECTION&gt;.names</c>.
/// </remarks>
public static class ModCredits
{
    /// <summary>Body layout of a section, determining how its <c>.names</c> value is parsed.</summary>
    public enum Layout
    {
        /// <summary>One name per line.</summary>
        Names = 1,
        /// <summary>One <c>Role||Name</c> pair per line, rendered as two columns.</summary>
        Roles = 2,
        /// <summary>One name per line, dealt round-robin across three columns.</summary>
        Columns3 = 3,
    }

    /// <summary>A single credits section belonging to a mod.</summary>
    /// <param name="Name">
    /// Section id; combined with the mod id to form the loc keys
    /// <c>&lt;MODID&gt;-&lt;NAME&gt;.header</c> / <c>.names</c>.
    /// </param>
    /// <param name="Kind">How the section body is laid out.</param>
    public record Section(string Name, Layout Kind = Layout.Names);

    /// <summary>A registered mod and its sections, in registration order.</summary>
    internal record Entry(string ModId, List<Section> Sections);

    /// <summary>All registered mods, rendered in the order they registered.</summary>
    internal static readonly List<Entry> Entries = [];

    /// <summary>
    /// Registers sections for the mod whose root namespace matches <typeparamref name="TFromMod"/>.
    /// Pass any type from your mod assembly (plugin class, a card model, etc.).
    /// </summary>
    /// <typeparam name="TFromMod">A type in your mod; its root namespace becomes the mod id.</typeparam>
    /// <param name="sections">The sections to display for this mod.</param>
    public static void Register<TFromMod>(params Section[] sections)
        => Entries.Add(new Entry(IdOf(typeof(TFromMod)), [.. sections]));

    /// <summary>Registers sections under an explicit, upper-cased mod id.</summary>
    /// <param name="modId">The mod id used to namespace loc keys.</param>
    /// <param name="sections">The sections to display for this mod.</param>
    public static void Register(string modId, params Section[] sections)
        => Entries.Add(new Entry(modId.ToUpperInvariant(), [.. sections]));

    /// <summary>
    /// Derives the mod id from a type's root namespace, matching the prefix
    /// BaseLib assigns to content ids (minus its trailing dash).
    /// </summary>
    private static string IdOf(Type type)
        => type.GetRootNamespace().ToUpperInvariant();

    /// <summary>Looks up a key in the vanilla <c>credits</c> loc table.</summary>
    internal static string Resolve(string key)
        => new LocString("credits", key).GetRawText();
}

/// <summary>
/// Harmony patch that injects every mod registered in <see cref="ModCredits"/>
/// into the credits screen, after the last vanilla section and before the engine logos.
/// </summary>
[HarmonyPatch(typeof(NCreditsScreen), "_Ready")]
public static class ModCreditsPatch
{
    private static readonly Color BannerColor  = new(1f, 0.55f, 0.20f);    // orange
    private static readonly Color ModNameColor = new(0.53f, 0.81f, 0.92f); // blue

    /// <summary>Builds and inserts the modded credit blocks once the screen is ready.</summary>
    [HarmonyPostfix]
    public static void Postfix(NCreditsScreen __instance)
    {
        if (ModCredits.Entries.Count == 0) return;

        var vbox = __instance
            .GetNodeOrNull<Control>("%ScreenContents")?
            .GetNodeOrNull<VBoxContainer>("VBoxContainer");
        if (vbox == null) return;

        var headerTpl = vbox.GetNodeOrNull<MegaLabel>("ModdingSupportHeader");
        var namesTpl  = vbox.GetNodeOrNull<MegaRichTextLabel>("ModdingSupportNames");
        var rolesCTpl = vbox.GetNodeOrNull<HBoxContainer>("zhsContainer");
        var multiCTpl = vbox.GetNodeOrNull<HBoxContainer>("PlaytestersContainer");
        if (headerTpl == null || namesTpl == null) return;

        var anchor = vbox.GetNodeOrNull<Control>("Spacer3");
        var at = anchor?.GetIndex() ?? vbox.GetChildCount();

        BuildHeader(vbox, ref at, ModCredits.Resolve("BASELIB-BANNER.name"), BannerColor, headerTpl, "Banner", null, 60);

        var m = 0;
        foreach (var e in ModCredits.Entries)
        {
            BuildHeader(vbox, ref at, ModCredits.Resolve($"{e.ModId}-{e.ModId}.title"), ModNameColor, headerTpl, "Mod" + m, null, 44);

            var s = 0;
            foreach (var sec in e.Sections)
            {
                var tag  = "M" + m + "S" + s;
                var head = ModCredits.Resolve(e.ModId + "-" + sec.Name.ToUpperInvariant() + ".header");
                var body = ModCredits.Resolve(e.ModId + "-" + sec.Name.ToUpperInvariant() + ".names");

                BuildHeader(vbox, ref at, head, null, headerTpl, tag + "H", 60f);

                switch (sec.Kind)
                {
                    case ModCredits.Layout.Roles when rolesCTpl != null:
                        BuildRoles(vbox, ref at, body, rolesCTpl, tag);
                        break;
                    case ModCredits.Layout.Columns3 when multiCTpl != null:
                        BuildMulti(vbox, ref at, body, multiCTpl, tag);
                        break;
                    case ModCredits.Layout.Names:
                    default:
                        BuildNames(vbox, ref at, body, namesTpl, tag);
                        break;
                }
                s++;
            }
            m++;
        }
    }

    /// <summary>
    /// Clones the header template and inserts it, optionally overriding vertical
    /// spacing (<paramref name="minHeight"/>) and text size (<paramref name="fontSize"/>).
    /// </summary>
    private static void BuildHeader(VBoxContainer vbox, ref int at, string? text,
        Color? color, MegaLabel tpl, string tag, float? minHeight = null, int? fontSize = null)
    {
        var h = (MegaLabel)tpl.Duplicate();
        h.UniqueNameInOwner = false;
        h.Name = "Mod_" + tag;
        h.Text = text ?? "";
        if (color.HasValue) h.AddThemeColorOverride("font_color", color.Value);
        if (minHeight.HasValue)
            h.CustomMinimumSize = new Vector2(h.CustomMinimumSize.X, minHeight.Value);
        if (fontSize.HasValue)
        {
            h.MaxFontSize = fontSize.Value;
            h.AddThemeFontSizeOverride("font_size", fontSize.Value);
        }
        vbox.AddChild(h);
        vbox.MoveChild(h, at++);
    }

    /// <summary>Renders a single-column list of names (one per line).</summary>
    private static void BuildNames(VBoxContainer vbox, ref int at, string? body,
        MegaRichTextLabel tpl, string tag)
    {
        var n = (MegaRichTextLabel)tpl.Duplicate();
        n.UniqueNameInOwner = false;
        n.Name = "Mod_" + tag + "_N";
        n.Text = body ?? "";
        vbox.AddChild(n);
        vbox.MoveChild(n, at++);
    }

    /// <summary>Renders a two-column roles/names block from <c>Role||Name</c> lines.</summary>
    private static void BuildRoles(VBoxContainer vbox, ref int at, string? body,
        HBoxContainer tpl, string tag)
    {
        var c = (HBoxContainer)tpl.Duplicate();
        c.UniqueNameInOwner = false;
        c.Name = "Mod_" + tag + "_C";

        var labels = new List<MegaRichTextLabel>();
        foreach (var child in c.GetChildren())
            if (child is MegaRichTextLabel lbl) labels.Add(lbl);
        if (labels.Count < 2) { c.QueueFree(); return; }

        var roles = new List<string>();
        var names = new List<string>();
        foreach (var line in (body ?? "").Split('\n'))
        {
            var parts = line.Split(["||"], StringSplitOptions.None);
            if (parts.Length != 2) continue;
            roles.Add(parts[0].Trim());
            names.Add(parts[1].Trim());
        }

        labels[0].UniqueNameInOwner = false;  // roles column (blue)
        labels[0].Name = "Mod_" + tag + "_Roles";
        labels[0].Text = string.Join("\n", roles);
        labels[1].UniqueNameInOwner = false;  // names column (cream)
        labels[1].Name = "Mod_" + tag + "_Names";
        labels[1].Text = string.Join("\n", names);

        vbox.AddChild(c);
        vbox.MoveChild(c, at++);
    }

    /// <summary>Renders names across the three columns of the playtester template.</summary>
    private static void BuildMulti(VBoxContainer vbox, ref int at, string? body,
        HBoxContainer tpl, string tag)
    {
        var c = (HBoxContainer)tpl.Duplicate();
        c.UniqueNameInOwner = false;
        c.Name = "Mod_" + tag + "_C";

        var columns = new List<MegaRichTextLabel>();
        foreach (var child in c.GetChildren())
            if (child is MegaRichTextLabel lbl) columns.Add(lbl);
        if (columns.Count == 0) { c.QueueFree(); return; }

        var names = (body ?? "").Split('\n');
        var buckets = new List<string>[columns.Count];
        for (var i = 0; i < columns.Count; i++) buckets[i] = [];
        for (var i = 0; i < names.Length; i++)
            buckets[i % columns.Count].Add(names[i]);

        for (var i = 0; i < columns.Count; i++)
        {
            columns[i].UniqueNameInOwner = false;
            columns[i].Name = "Mod_" + tag + "_C" + i;
            columns[i].Text = string.Join("\n", buckets[i]);
        }

        vbox.AddChild(c);
        vbox.MoveChild(c, at++);
    }
}