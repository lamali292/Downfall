using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx.Forms;

namespace Downfall.DownfallCode.Utils;

/// <summary>Which spine bone the vanilla Form VFX (Void/Serpent/Reaper/Echo) attaches to, per character.</summary>
public static class FormBoneRegistry
{
    private static readonly Dictionary<(Type Form, Type Character), string> Bones = new();

    /// <summary>Attaches vanilla form VFX <typeparamref name="TForm" /> (e.g. <c>NVoidFormVfx</c>) to <paramref name="bone" /> for <typeparamref name="TCharacter" />.</summary>
    public static void Register<TForm, TCharacter>(string bone)
        where TForm : NFormVfx
        where TCharacter : CharacterModel
    {
        Bones[(typeof(TForm), typeof(TCharacter))] = bone;
    }

    public static bool TryGet(Type formType, Type characterType, out string? boneName)
    {
        return Bones.TryGetValue((formType, characterType), out boneName);
    }
}
