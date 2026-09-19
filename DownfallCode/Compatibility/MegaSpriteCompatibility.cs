using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;

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

    /// <summary>
    ///     The Creature this sprite's spine controller is displaying, found by walking up the scene
    ///     tree to the owning NCreature. GenerateAnimator/SetupCustomAnimationStates only receive the
    ///     MegaSprite controller (no Creature), so per-instance conditions (low HP, stance, ...) need
    ///     this instead. Returns null if the sprite isn't parented under an NCreature yet.
    /// </summary>
    public static Creature? GetOwningCreature(this MegaSprite sprite)
    {
        for (var node = sprite.BoundObject as Node; node != null; node = node.GetParentOrNull<Node>())
        {
            if (node is NCreature creature)
                return creature.Entity;
        }

        return null;
    }
}