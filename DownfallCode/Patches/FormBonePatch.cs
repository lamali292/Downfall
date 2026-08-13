using System.Reflection;
using Downfall.DownfallCode.Utils;
using Godot;
using HarmonyLib;
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
    
    private static bool Prefix(object __instance, object spineSprite, Node2D sourceNode,
        MethodBase __originalMethod)
    {
        var type = __instance.GetType();

        var owner = AccessTools.Field(type, "_owner")?.GetValue(__instance);
        var character = owner == null ? null : GetMember(owner, "Character");
        if (character == null) return true; 

        var formName = __originalMethod.DeclaringType!.FullName!;
        if (!FormBoneRegistry.TryGet(formName, character.GetType(), out var boneName) || boneName == null)
            return true;
        
        var megaSpriteType = AccessTools.TypeByName("MegaCrit.Sts2.Core.Bindings.MegaSpine.MegaSprite");
        
        var copier = AccessTools.Field(type, "_spineCopier")?.GetValue(__instance);
        if (copier != null)
            AccessTools.Method(copier.GetType(), "Initialize", [megaSpriteType, typeof(Node2D)])
                ?.Invoke(copier, [spineSprite, sourceNode]);

        // The follower's SetSpineSprite is overloaded; the Type[] picks (MegaSprite, string).
        var follower = AccessTools.Field(type, "_boneFollower")?.GetValue(__instance);
        if (follower != null)
            AccessTools.Method(follower.GetType(), "SetSpineSprite", [megaSpriteType, typeof(string)])
                ?.Invoke(follower, [spineSprite, boneName]);

        return false; // bone follower called exactly once
    }

    private static object? GetMember(object obj, string name)
    {
        var t = obj.GetType();
        var prop = AccessTools.Property(t, name);
        return prop != null ? prop.GetValue(obj) : AccessTools.Field(t, name)?.GetValue(obj);
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