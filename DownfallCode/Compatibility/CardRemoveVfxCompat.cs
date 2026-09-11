using System.Reflection;
using Downfall.DownfallCode.Utils;
using Godot;

namespace Downfall.DownfallCode.Compatibility;

public static class CardRemoveVfxCompat
{
    private static readonly Type? VfxType = VersionCompat.FindType(
        "MegaCrit.Sts2.Core.Nodes.Vfx.Cards.NCardRemoveVfx");

    private static readonly MethodInfo? CreateMethod =
        VersionCompat.FindStaticMethod(VfxType, "Create");

    private static readonly FieldInfo? DeleteCardDelayField =
        VersionCompat.FindStaticField(VfxType, "deleteCardDelay");

    public static bool IsAvailable => VfxType != null;

    public static double DeleteCardDelay =>
        VersionCompat.GetStaticField(DeleteCardDelayField, fallback: 0.0);

    /// <summary>
    /// Creates and returns a NCardRemoveVfx node for the given card, or null
    /// if this game version doesn't have the class.
    /// </summary>
    public static Node? Create(Node cardNode)
    {
        return VersionCompat.InvokeStatic<Node>(CreateMethod, cardNode);
    }
}