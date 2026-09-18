using Godot;
using MegaCrit.Sts2.Core.Localization;

namespace Downfall.DownfallCode.Voting;

/// <summary>
/// Shared look-and-feel for the voting feature's popups - a plain Button
/// with a hand-drawn StyleBoxFlat instead of a borrowed game texture. This
/// avoids both the odd fixed "flag" shape of the base game's confirm/cancel
/// button art and the shared-resource hover bug that art carried (every
/// instance of that button lit up together on hover, since its outline
/// material wasn't marked resource_local_to_scene).
/// </summary>
public static class VotingUi
{
    private const string LocTable = "voting_ui";

    public static string Loc(string key) => new LocString(LocTable, key).GetFormattedText();

    public static string Loc(string key, params (string Name, object Value)[] vars)
    {
        var loc = new LocString(LocTable, key);
        foreach (var (name, value) in vars)
            loc.AddObj(name, value);
        return loc.GetFormattedText();
    }

    public static Button CreateActionButton(string text, bool primary)
    {
        var button = new Button { Text = text, CustomMinimumSize = new Vector2(150, 46) };
        StyleActionButton(button, primary);
        return button;
    }

    public static void StyleActionButton(Button button, bool primary)
    {
        var accent = primary ? new Color(0.55f, 0.78f, 0.55f) : new Color(0.82f, 0.5f, 0.45f);
        var cream = new Color(1f, 0.964706f, 0.886275f);
        var bg = new Color(0.16f, 0.21f, 0.23f, 0.95f);

        button.AddThemeFontOverride("font", GD.Load<FontVariation>("res://themes/kreon_regular_shared.tres"));
        button.AddThemeFontSizeOverride("font_size", 20);
        foreach (var state in new[] { "font_color", "font_hover_color", "font_pressed_color", "font_focus_color" })
            button.AddThemeColorOverride(state, cream);

        button.AddThemeStyleboxOverride("normal", MakeButtonStyle(bg, accent, 2));
        button.AddThemeStyleboxOverride("hover", MakeButtonStyle(bg.Lerp(accent, 0.25f), accent, 2));
        button.AddThemeStyleboxOverride("pressed", MakeButtonStyle(bg.Lerp(Colors.Black, 0.3f), accent, 2));
        button.AddThemeStyleboxOverride("focus", MakeButtonStyle(bg.Lerp(accent, 0.25f), accent, 3));
        button.AddThemeStyleboxOverride("disabled", MakeButtonStyle(bg.Lerp(Colors.Black, 0.4f), accent.Lerp(Colors.Black, 0.5f), 2));
    }

    /// <summary>
    /// A toggleable variant of <see cref="StyleActionButton"/> - same panel
    /// look, but with a distinct "pressed and staying pressed" style so it
    /// reads as a selectable chip (report reasons, filters) rather than a
    /// one-shot action.
    /// </summary>
    public static void StyleToggleButton(Button button)
    {
        button.ToggleMode = true;

        var accent = new Color(1f, 0.6f, 0.3f);
        var cream = new Color(1f, 0.964706f, 0.886275f);
        var bg = new Color(0.16f, 0.21f, 0.23f, 0.95f);

        button.AddThemeFontOverride("font", GD.Load<FontVariation>("res://themes/kreon_regular_shared.tres"));
        button.AddThemeFontSizeOverride("font_size", 18);
        foreach (var state in new[] { "font_color", "font_hover_color", "font_pressed_color", "font_focus_color" })
            button.AddThemeColorOverride(state, cream);

        button.AddThemeStyleboxOverride("normal", MakeButtonStyle(bg, new Color(0.35f, 0.4f, 0.44f), 2));
        button.AddThemeStyleboxOverride("hover", MakeButtonStyle(bg.Lerp(accent, 0.15f), accent, 2));
        button.AddThemeStyleboxOverride("pressed", MakeButtonStyle(bg.Lerp(accent, 0.35f), accent, 3));
        button.AddThemeStyleboxOverride("hover_pressed", MakeButtonStyle(bg.Lerp(accent, 0.45f), accent, 3));
        button.AddThemeStyleboxOverride("focus", MakeButtonStyle(bg.Lerp(accent, 0.15f), accent, 2));
    }

    private static StyleBoxFlat MakeButtonStyle(Color bg, Color border, int borderWidth)
    {
        return new StyleBoxFlat
        {
            BgColor = bg,
            BorderColor = border,
            BorderWidthTop = borderWidth,
            BorderWidthBottom = borderWidth,
            BorderWidthLeft = borderWidth,
            BorderWidthRight = borderWidth,
            CornerRadiusTopLeft = 6,
            CornerRadiusTopRight = 6,
            CornerRadiusBottomLeft = 6,
            CornerRadiusBottomRight = 6,
            ContentMarginLeft = 18,
            ContentMarginRight = 18,
            ContentMarginTop = 8,
            ContentMarginBottom = 8,
        };
    }

    public static Label ThemedLabel(string text, FontVariation font, Color color, int fontSize)
    {
        var label = new Label { Text = text };
        label.AddThemeFontOverride("font", font);
        label.AddThemeFontSizeOverride("font_size", fontSize);
        label.AddThemeColorOverride("font_color", color);
        return label;
    }

    /// <summary>
    /// Opens the operating system's own file picker rather than Godot's
    /// built-in in-engine FileDialog control, where available.
    /// </summary>
    public static void PickImageFile(Node owner, Action<string> onSelected)
    {
        if (DisplayServer.HasFeature(DisplayServer.Feature.NativeDialogFile))
        {
            DisplayServer.FileDialogShow(
                Loc("DOWNFALL-VOTING.choose_image_dialog_title"),
                "",
                "",
                false,
                DisplayServer.FileDialogMode.OpenFile,
                ["*.png,*.jpg,*.jpeg,*.webp;Image Files"],
                Callable.From<bool, string[], int>((status, paths, _) =>
                {
                    if (status && paths.Length > 0)
                        onSelected(paths[0]);
                }));
            return;
        }

        // Fallback for platforms without a native file dialog implementation.
        var dialog = new FileDialog
        {
            Title = Loc("DOWNFALL-VOTING.choose_image_dialog_title"),
            FileMode = FileDialog.FileModeEnum.OpenFile,
            Access = FileDialog.AccessEnum.Filesystem,
        };
        dialog.AddFilter("*.png,*.jpg,*.jpeg,*.webp", "Images");

        dialog.FileSelected += path =>
        {
            onSelected(path);
            dialog.QueueFree();
        };
        dialog.Canceled += dialog.QueueFree;

        owner.AddChild(dialog);
        dialog.PopupCenteredRatio(0.55f);
    }
}
