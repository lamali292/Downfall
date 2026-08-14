using System.Reflection;
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace Downfall.DownfallCode.Compatibility;

public static class MegaSpriteExtensions
{
    /// <summary>
    ///     Cross-version global bone transform. On builds whose Spine binding lacks
    ///     'get_global_bone_transform', returns null instead of throwing.
    /// </summary>
    public static Transform2D? GetGlobalBoneTransformCompat(this MegaSprite sprite, string boneName)
    {
        if (sprite is null)
            return null;

        // The native SpineSprite object underneath the binding.
        var native = sprite.BoundObject;
        if (native is null || !native.HasMethod("get_global_bone_transform"))
            return null;

        var result = native.Call("get_global_bone_transform", boneName);
        return result.VariantType == Variant.Type.Object || result.VariantType == Variant.Type.Nil
            ? null
            : result.As<Transform2D>();
    }
}