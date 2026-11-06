using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.DownfallCode.Compatibility;

public static class MegaSpriteExtensions
{
    /// <summary>
    ///     Cross-version global bone transform. On builds whose Spine binding lacks
    ///     'get_global_bone_transform', or when the named bone doesn't exist on this skeleton
    ///     at all (e.g. off-class rigs without a character-specific bone), returns null instead
    ///     of throwing or silently returning a wrong transform.
    /// </summary>
    /// <remarks>
    ///     'get_global_bone_transform' does NOT return Nil for a missing bone - it returns a
    ///     valid-looking Transform2D anyway (observed sitting at the skeleton root), so a missing
    ///     bone can't be detected from its result. Bone existence has to be checked separately via
    ///     Spine's 'find_bone' (<see cref="MegaSkeleton.FindBone" />), which does null-check correctly.
    /// </remarks>
    public static Transform2D? GetGlobalBoneTransformCompat(this MegaSprite sprite, string boneName)
    {
        if (sprite is null)
            return null;

        if (sprite.GetSkeleton()?.FindBone(boneName) == null)
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