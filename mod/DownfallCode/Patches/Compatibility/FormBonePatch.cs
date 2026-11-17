using Downfall.DownfallCode.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Vfx.Forms;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

namespace Downfall.DownfallCode.Patches;

/// <summary>
///     Attaches a vanilla Form VFX to the bone registered for the owner's character (see <see cref="FormBoneRegistry" />)
///     instead of the hardcoded vanilla one. Characters without a registered bone keep the vanilla behavior.
///     <c>_owner</c> is a protected field of the base <see cref="NFormVfx" /> (Harmony's <c>___name</c> injection only
///     finds fields declared on the patched class itself), so it is read through a cached field reference; the
///     bone follower and spine copier are declared on the form classes and are injected.
/// </summary>
internal static class FormBoneHelper
{
    private static readonly AccessTools.FieldRef<NFormVfx, Player?> OwnerRef =
        AccessTools.FieldRefAccess<NFormVfx, Player?>("_owner");

    /// <returns>false to skip the vanilla method (the bone was attached here), true to run vanilla.</returns>
    public static bool TryAttach(Type formType, NFormVfx form, NSpineSpriteBoneFollower? follower,
        NSpineSpriteCopier? copier, MegaSprite spineSprite, Node2D sourceNode)
    {
        var character = OwnerRef(form)?.Character;
        if (character == null || !FormBoneRegistry.TryGet(formType, character.GetType(), out var boneName) ||
            boneName == null)
            return true;

        copier?.Initialize(spineSprite, sourceNode);
        follower?.SetSpineSprite(spineSprite, boneName);
        return false;
    }
}

[HarmonyPatch(typeof(NVoidFormVfx), "SetSpineSprite")]
internal static class VoidFormBonePatch
{
    private static bool Prefix(NFormVfx __instance, MegaSprite spineSprite, Node2D sourceNode,
        NSpineSpriteBoneFollower? ____boneFollower) =>
        FormBoneHelper.TryAttach(typeof(NVoidFormVfx), __instance, ____boneFollower, null, spineSprite, sourceNode);
}

[HarmonyPatch(typeof(NSerpentFormVfx), "SetSpineSprite")]
internal static class SerpentFormBonePatch
{
    private static bool Prefix(NFormVfx __instance, MegaSprite spineSprite, Node2D sourceNode,
        NSpineSpriteBoneFollower? ____boneFollower) =>
        FormBoneHelper.TryAttach(typeof(NSerpentFormVfx), __instance, ____boneFollower, null, spineSprite, sourceNode);
}

[HarmonyPatch(typeof(NReaperFormVfx), "SetSpineSprite")]
internal static class ReaperFormBonePatch
{
    private static bool Prefix(NFormVfx __instance, MegaSprite spineSprite, Node2D sourceNode,
        NSpineSpriteBoneFollower? ____boneFollower) =>
        FormBoneHelper.TryAttach(typeof(NReaperFormVfx), __instance, ____boneFollower, null, spineSprite, sourceNode);
}

[HarmonyPatch(typeof(NEchoFormVfx), "SetSpineSprite")]
internal static class EchoFormBonePatch
{
    private static bool Prefix(NFormVfx __instance, MegaSprite spineSprite, Node2D sourceNode,
        NSpineSpriteBoneFollower? ____boneFollower, NSpineSpriteCopier? ____spineCopier) =>
        FormBoneHelper.TryAttach(typeof(NEchoFormVfx), __instance, ____boneFollower, ____spineCopier, spineSprite, sourceNode);
}
