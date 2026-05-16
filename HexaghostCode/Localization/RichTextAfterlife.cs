using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.RichTextTags;

namespace Hexaghost.HexaghostCode.Localization;

public partial class RichTextAfterlife : AbstractMegaRichTextEffect
{
    protected override string Bbcode => "afterlife";

    public override bool _ProcessCustomFX(CharFXTransform charFx)
    {
        charFx.Color = new Color("#78D1A0");
        return true;
    }
}

[HarmonyPatch(typeof(MegaRichTextLabel), "InstallEffectsIfNeeded")]
public static class AddCustomTextEffectsPatch
{
    [HarmonyPostfix]
    public static void AddCustomEffects(MegaRichTextLabel __instance)
    {
        __instance.CustomEffects.Add(new RichTextAfterlife());
    }
}