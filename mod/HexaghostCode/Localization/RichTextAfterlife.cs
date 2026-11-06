using Downfall.DownfallCode.Config;
using Godot;
using MegaCrit.Sts2.Core.RichTextTags;

namespace Hexaghost.HexaghostCode.Localization;

public partial class RichTextAfterlife : AbstractMegaRichTextEffect
{
    protected override string Bbcode => "afterlife";

    public override bool _ProcessCustomFX(CharFXTransform charFx)
    {
        charFx.Color = DownfallConfig.AfterlifeColor;
        return true;
    }
}