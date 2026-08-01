using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Utils;


public static class FormBoneRegistry
{
    private const string Ns = "MegaCrit.Sts2.Core.Nodes.Vfx.Forms.";
    private static readonly Dictionary<(string Form, Type Character), string> Bones = new();

    public static void RegisterVoidForm<TCharacter>(string bone)    where TCharacter : CharacterModel => Register(Ns + "NVoidFormVfx",    typeof(TCharacter), bone);
    public static void RegisterSerpentForm<TCharacter>(string bone) where TCharacter : CharacterModel => Register(Ns + "NSerpentFormVfx", typeof(TCharacter), bone);
    public static void RegisterReaperForm<TCharacter>(string bone)  where TCharacter : CharacterModel => Register(Ns + "NReaperFormVfx",  typeof(TCharacter), bone);
    public static void RegisterEchoForm<TCharacter>(string bone)    where TCharacter : CharacterModel => Register(Ns + "NEchoFormVfx",    typeof(TCharacter), bone);

    public static void Register(string formTypeName, Type characterType, string boneName)
        => Bones[(formTypeName, characterType)] = boneName;

    public static bool TryGet(string formTypeName, Type characterType, out string? boneName)
        => Bones.TryGetValue((formTypeName, characterType), out boneName);
}

/*
public static class FormBoneRegistry
{
    private static readonly Dictionary<(Type Form, Type Character), string> Bones = new();


    public static void RegisterVoidForm<TCharacter>(string boneName)
        where TCharacter : CharacterModel
    {
        Register<NVoidFormVfx, TCharacter>(boneName);
    }
    
    public static void RegisterSerpentForm<TCharacter>(string boneName)
        where TCharacter : CharacterModel
    {
        Register<NSerpentFormVfx, TCharacter>(boneName);
    }
    
    public static void RegisterReaperForm<TCharacter>(string boneName)
        where TCharacter : CharacterModel
    {
        Register<NReaperFormVfx, TCharacter>(boneName);
    }
    
    public static void RegisterEchoForm<TCharacter>(string boneName)
        where TCharacter : CharacterModel
    {
        Register<NEchoFormVfx, TCharacter>(boneName);
    }

    
    public static void Register<TForm, TCharacter>(string boneName)
        where TForm : Node2D
        where TCharacter : CharacterModel
        => Bones[(typeof(TForm), typeof(TCharacter))] = boneName;

    public static bool TryGet(Type formType, Type characterType, out string? boneName)
        => Bones.TryGetValue((formType, characterType), out boneName);
}*/