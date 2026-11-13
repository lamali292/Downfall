using System.Reflection;
using BaseLib.Config;
using BaseLib.Config.UI;
using Godot;

namespace Downfall.DownfallCode.Config.UI;

/// <summary>
/// A BaseLib color swatch with a "reset to default" button beside it, for a static <see cref="Color"/> config property.
/// Composes the existing <see cref="NConfigColorPicker"/> rather than reimplementing it; the reset goes through
/// <see cref="ModConfig.ConfigReloaded"/> so the swatch (and any other listener) refreshes itself.
/// </summary>
public partial class NResettableColorPicker : HBoxContainer
{
    // Same footprint as the other config controls (e.g. the tickbox), so the swatch lines up with them.
    private const float ControlWidth = 324;
    private const float ResetSlotWidth = 44;

    public NResettableColorPicker(ModConfig config, PropertyInfo property, string resetLabel)
    {
        CustomMinimumSize = new Vector2(ControlWidth, 64);
        SizeFlagsHorizontal = SizeFlags.ShrinkEnd;
        SizeFlagsVertical = SizeFlags.Fill;
        MouseFilter = MouseFilterEnum.Ignore;

        // Empty slot mirroring the reset button, so the swatch stays in the middle of the control.
        AddChild(new Control { CustomMinimumSize = new Vector2(ResetSlotWidth, 0), MouseFilter = MouseFilterEnum.Ignore });

        var swatch = new NConfigColorPicker();
        swatch.Initialize(config, property);
        swatch.CustomMinimumSize = new Vector2(0, 64);
        swatch.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        AddChild(swatch);

        AddChild(new NResetIconButton(() => Reset(config, property)) { TooltipText = resetLabel });
    }

    private static void Reset(ModConfig config, PropertyInfo property)
    {
        var defaultValue = config.GetDefaultValue<object?>(property.Name);
        if (defaultValue == null) return;

        property.SetValue(null, defaultValue);
        config.Save();
        config.ConfigReloaded();
        config.Changed();
    }
}
