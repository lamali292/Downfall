using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Vfx;

[GlobalClass]
public partial class NSlimeCreatureVisuals : NCreatureVisuals
{
    private readonly List<(MegaBone Bone, Node2D Node)> _attachments = [];

    public override void _Ready()
    {
        base._Ready();
        var premultMat = new CanvasItemMaterial
        {
            BlendMode = CanvasItemMaterial.BlendModeEnum.PremultAlpha
        };
        if (SpineBody != null)
            SpineBody.SetNormalMaterial(premultMat);
        else
            GetCurrentBody().Material = premultMat;

        GetTree().ProcessFrame += SetupBones;
    }

    private void SetupBones()
    {
        GetTree().ProcessFrame -= SetupBones;

        var skeleton = SpineBody?.GetSkeleton();
        TryAttach(skeleton, "eyeback1", "%LeftStick");
        TryAttach(skeleton, "eyeback4", "%RightStick");
        TryAttach(skeleton, "eyeshadow", "%ScrapVfx");
        TryAttach(skeleton, "bone7", "%Antennae");
        TryAttach(skeleton, "bone4", "%Stopwatch");
        TryAttach(skeleton, "bone8", "%Crown");
        TryAttach(skeleton, "eye", "%Eye");

        /*
        var data = skeleton?.GetData();
        if (data != null)
        {
            var bones = data.BoundObject.Call("get_bones").AsGodotArray();
            foreach (var bone in bones)
            {
                var boneObj = bone.AsGodotObject();
                SlimeBossMainFile.Logger.Info($"Bone: {boneObj.Call("get_bone_name")}");
            }
        }*/
        
        SpineBody?.ConnectWorldTransformsChanged(Callable.From<Variant>(OnWorldTransformsChanged));
    }

    private void TryAttach(MegaSkeleton? skeleton, string boneName, string nodePath)
    {
        var node = GetNodeOrNull<Node2D>(nodePath);
        if (node == null) return;

        var bone = skeleton?.FindBone(boneName);
        if (bone == null) return;

        _attachments.Add((bone, node));
    }

    private void OnWorldTransformsChanged(Variant _)
    {
        foreach (var (bone, node) in _attachments)
        {
            var x = bone.BoundObject.Call("get_world_x").As<float>();
            var y = bone.BoundObject.Call("get_world_y").As<float>();
            node.Position = new Vector2(x, y);
        }
    }
}






public static class MySlimeDeathIsolationPatches
{
    // A private tracking set completely isolated to your own mod's lifecycle
    private static readonly HashSet<NCreature> DyingSlimes = new();

    [HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
    public static class SlimeOnlyDeathAnimStartPatch
    {
        public static void Prefix(NCreature __instance, ref bool shouldRemove)
        {
            if (__instance.Entity.Monster is not SlimeModel) return;
            DyingSlimes.Add(__instance);
            shouldRemove = true;
            NCombatRoom.Instance?.RemoveCreatureNode(__instance);
        }
    }

    [HarmonyPatch(typeof(NCreature), nameof(NCreature.GetCurrentAnimationTimeRemaining))]
    public static class SlimeOnlyDeathAnimTimePatch
    {
        public static bool Prefix(NCreature __instance, ref float __result)
        {
            if (!DyingSlimes.Contains(__instance))
                return true;
            DyingSlimes.Remove(__instance);
            __result = 0f;
            return false;
        }
    }
}