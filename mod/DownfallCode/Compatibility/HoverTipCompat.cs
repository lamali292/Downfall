using System.Reflection;
using MegaCrit.Sts2.Core.HoverTips;

namespace Downfall.DownfallCode.Compatibility;

public static class HoverTipCompat
{
    private static readonly FieldInfo? IdField = BackingField("Id");
    private static readonly FieldInfo? IsSmartField = BackingField("IsSmart");

    /// <summary>
    ///     Sets <c>Id</c>/<c>IsSmart</c> on a tip. Older builds expose setters, newer ones made both get-only, so the
    ///     auto-property backing fields (present in both) are written on a boxed copy.
    /// </summary>
    public static HoverTip WithIdAndSmart(this HoverTip tip, string id, bool isSmart)
    {
        object boxed = tip;
        IdField?.SetValue(boxed, id);
        IsSmartField?.SetValue(boxed, isSmart);
        return (HoverTip)boxed;
    }

    private static FieldInfo? BackingField(string property) =>
        typeof(HoverTip).GetField($"<{property}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
}
