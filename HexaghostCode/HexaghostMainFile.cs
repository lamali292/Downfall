using System.Reflection;
using BaseLib.Utils;
using Downfall.DownfallCode.Patches;
using Downfall.DownfallCode.Utils;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using Hexaghost.HexaghostCode.CustomEnums;
using Hexaghost.HexaghostCode.Events;
using Hexaghost.HexaghostCode.Localization;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode;

[ModInitializer(nameof(Initialize))]
public partial class HexaghostMainFile : Node
{
    public const string ModId = "Hexaghost"; //At the moment, this is used only for the Logger and harmony names.

    //public static Logger Logger { get; } =new(ModId, LogType.Generic);

    public static void Initialize()
    {
        RichTextEffectRegistry.Register<RichTextAfterlife>();
        CustomLocTableManager.Register("ghostflames");
        HexaghostSubscriber.Subscribe();
        var assembly = Assembly.GetExecutingAssembly();
        ScriptManagerBridge.LookupScriptsInAssembly(assembly);
        Harmony harmony = new(ModId);
        harmony.PatchAll();
    }
}

[HarmonyPatch(typeof(ModelDb), "InitIds")]
internal static class ModelDbInitIdsPatch
{
    [HarmonyPostfix]
    private static void LogRegisteredCounts()
    {
        CardKeywordSubRegistry.Register(CardKeyword.Ethereal, HexaghostKeyword.Afterlife);
    }
}

[HarmonyPatch(typeof(CardKeywordExtensions), nameof(CardKeywordExtensions.GetCardText))]
public static class CardKeywordColorPatch
{
    public static void Postfix(CardKeyword keyword, ref string __result)
    {
        string? color = null;
        if (keyword == HexaghostKeyword.Afterlife) color = "afterlife";
        if (color == null) return;
        __result = __result.Replace("[gold]", $"[{color}]")
            .Replace("[/gold]", $"[/{color}]");
    }
}