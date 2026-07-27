namespace Downfall.DownfallCode.Patches;

using System;
using System.Collections.Generic;
using BaseLib.Extensions;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Screens.Credits;

// =====================================================================
//  PUBLIC API  -  other mods call this at load time to add credits.
//
//  All keys live in the vanilla "credits" loc table, namespaced by mod id:
//      <MODID>-<SECTION>.header   and   <MODID>-<SECTION>.names
//  e.g. registering from the Downfall namespace with Section("TEAM")
//  looks up "DOWNFALL-TEAM.header" / "DOWNFALL-TEAM.names".
//
//  Register (id derived from your namespace, recommended) - pass any type
//  from your mod assembly (plugin class, a card model, etc.):
//      ModCredits.Register<DownfallMainFile>(
//          new ModCredits.Section("TEAM"),
//          new ModCredits.Section("ART", ModCredits.Layout.Roles),
//          new ModCredits.Section("LOC", ModCredits.Layout.Columns3));
//
//  Register (explicit id) - if you'd rather name it yourself:
//      ModCredits.Register("Downfall",
//          new ModCredits.Section("TEAM"));
//
//  Body (.names) format by layout:
//      Names     (1) - one name per line
//      Roles     (2) - one "Role||Name" pair per line
//      Columns3  (3) - one name per line, dealt across 3 columns
// =====================================================================
public static class ModCredits
{
    public enum Layout { Names = 1, Roles = 2, Columns3 = 3 }

    public record Section(string Name, Layout Kind = Layout.Names);

    internal record Entry(string ModId, List<Section> Sections);

    internal static readonly List<Entry> Entries = [];

    /// <summary>Register sections; the mod id is derived from T's root namespace.</summary>
    public static void Register<TFromMod>(params Section[] sections)
        => Entries.Add(new Entry(IdOf(typeof(TFromMod)), [.. sections]));

    /// <summary>Register sections with an explicit mod id.</summary>
    public static void Register(string modId, params Section[] sections)
        => Entries.Add(new Entry(modId.ToUpperInvariant(), [.. sections]));

    // Matches the DOWNFALL- namespace BaseLib gives content IDs, minus the
    // trailing dash (we add the '-' ourselves when building keys).
    private static string IdOf(Type type)
        => type.GetRootNamespace().ToUpperInvariant();

    internal static string Resolve(string key)
        => new LocString("credits", key).GetRawText();
}

// =====================================================================
//  RENDERER  -  injects everything registered above into the screen.
// =====================================================================
[HarmonyPatch(typeof(NCreditsScreen), "_Ready")]
public static class ModCreditsPatch
{
    // Shown ONCE above all modded content so it's clearly not vanilla.
    private static readonly Color BannerColor  = new(1f, 0.55f, 0.20f);    // orange
    private static readonly Color ModNameColor = new(0.53f, 0.81f, 0.92f); // blue

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

        // Master banner: makes the whole block unmistakably modded.
        BuildHeader(vbox, ref at, ModCredits.Resolve("BASELIB-BANNER.name"), BannerColor, headerTpl, "Banner", null, 60);

        var m = 0;
        foreach (var e in ModCredits.Entries)
        {
            // Which mod this block belongs to.
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