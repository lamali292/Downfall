using System.Reflection;
using Downfall.DownfallCode.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Vfx.Forms;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace Downfall.DownfallCode.Patches;

public static class FormBonePatcher
{
    private const string Ns = "MegaCrit.Sts2.Core.Nodes.Vfx.Forms.";
    private static readonly string[] FormTypeNames =
    {
        Ns + "NVoidFormVfx",
        Ns + "NSerpentFormVfx",
        Ns + "NReaperFormVfx",
        Ns + "NEchoFormVfx",
    };

    private const BindingFlags Ff =
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    public static void Apply(Harmony harmony, Logger logger)
    {
        var prefix = new HarmonyMethod(typeof(FormBonePatcher), nameof(Prefix));

        foreach (var name in FormTypeNames)
        {
            Type? formType;
            try { formType = AccessTools.TypeByName(name); }
            catch { formType = null; }

            if (formType == null)
            {
                logger.Info($"[FormBone] skip, type not present in this version: {name}");
                continue;
            }

            var method = AccessTools.DeclaredMethod(formType, "SetSpineSprite");
            if (method == null)
            {
                logger.Warn($"[FormBone] skip, no SetSpineSprite on {formType.Name}");
                continue;
            }

            try
            {
                harmony.Patch(method, prefix: prefix);
                logger.Info($"[FormBone] patched {formType.Name}");
            }
            catch (Exception ex)
            {
                logger.Error($"[FormBone] FAILED {formType.Name}: {ex.Message}");
            }
        }
    }

    // Single prefix for every form. Keyed off the method it was attached to,
    // so it needs no compile-time reference to any specific form type.
    private static bool Prefix(NFormVfx __instance, MegaSprite spineSprite, Node2D sourceNode,
                               MethodBase __originalMethod)
    {
        var owner = __instance._owner;
        if (owner?.Character == null) return true; // null-owner/test path -> original uses _testBoneName

        var formName = __originalMethod.DeclaringType!.FullName!;
        if (!FormBoneRegistry.TryGet(formName, owner.Character.GetType(), out var boneName) || boneName == null)
            return true; // basegame / unregistered -> run original untouched

        var type = __instance.GetType();

        // Echo runs a spine copier before the bone follower; other forms just don't have the field.
        if (type.GetField("_spineCopier", Ff)?.GetValue(__instance) is NSpineSpriteCopier copier)
            copier.Initialize(spineSprite, sourceNode);

        if (type.GetField("_boneFollower", Ff)?.GetValue(__instance) is NSpineSpriteBoneFollower follower)
            follower.SetSpineSprite(spineSprite, boneName);

        return false; // bone follower called exactly once
    }
}

/*
[HarmonyPatch(typeof(NVoidFormVfx), "SetSpineSprite")]
public static class VoidFormBonePatch
{
    private static bool Prefix(NVoidFormVfx __instance, MegaSprite spineSprite, Node2D sourceNode)
    {
        var owner = __instance._owner;
        if (owner?.Character == null) return true;

        if (!FormBoneRegistry.TryGet(typeof(NVoidFormVfx), owner.Character.GetType(), out var boneName))
            return true;

        if (__instance._boneFollower != null)
            __instance._boneFollower.SetSpineSprite(spineSprite, boneName);
        return false;
    }
}

[HarmonyPatch(typeof(NSerpentFormVfx), "SetSpineSprite")]
public static class SerpentFormBonePatch
{
    private static bool Prefix(NSerpentFormVfx __instance, MegaSprite spineSprite, Node2D sourceNode)
    {
        var owner = __instance._owner;
        if (owner?.Character == null) return true;

        if (!FormBoneRegistry.TryGet(typeof(NSerpentFormVfx), owner.Character.GetType(), out var boneName))
            return true;

        if (__instance._boneFollower != null)
            __instance._boneFollower.SetSpineSprite(spineSprite, boneName);
        return false;
    }
}

[HarmonyPatch(typeof(NReaperFormVfx), "SetSpineSprite")]
public static class ReaperFormBonePatch
{
    private static bool Prefix(NReaperFormVfx __instance, MegaSprite spineSprite, Node2D sourceNode)
    {
        var owner = __instance._owner;
        if (owner?.Character == null) return true;

        if (!FormBoneRegistry.TryGet(typeof(NReaperFormVfx), owner.Character.GetType(), out var boneName))
            return true;

        if (__instance._boneFollower != null)
            __instance._boneFollower.SetSpineSprite(spineSprite, boneName);
        return false;
    }
}

[HarmonyPatch(typeof(NEchoFormVfx), "SetSpineSprite")]
public static class EchoFormBonePatch
{
    private static bool Prefix(NEchoFormVfx __instance, MegaSprite spineSprite, Node2D sourceNode)
    {
        var owner = __instance._owner;
        if (owner?.Character == null) return true;

        if (!FormBoneRegistry.TryGet(typeof(NEchoFormVfx), owner.Character.GetType(), out var boneName))
            return true;

        if (__instance._spineCopier != null)
            __instance._spineCopier.Initialize(spineSprite, sourceNode);

        if (__instance._boneFollower != null)
            __instance._boneFollower.SetSpineSprite(spineSprite, boneName);
        return false;
    }
}
*/