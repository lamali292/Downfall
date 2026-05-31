using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace Downfall.DownfallCode.Artists;

public class ArtistHoverTip(LocString title, Texture2D? icon) : IHoverTip
{
    public LocString Title => title;
    public Texture2D? Icon => icon;
    public string Id => $"ArtistHoverTip:{title}";
    public bool IsSmart => false;
    public bool IsDebuff => false;
    public bool IsInstanced => false;
    public AbstractModel? CanonicalModel => null;
}

[HarmonyPatch(typeof(NHoverTipSet), "Init")]
internal static class NHoverTipSetInitPatch
{
    private static void Prefix(NHoverTipSet __instance, ref IEnumerable<IHoverTip> hoverTips)
    {
        var shiftHeld = true; //Input.IsPhysicalKeyPressed(Key.Shift);
        var list = hoverTips.ToList();

        hoverTips = list
            .Where(tip => tip is not ArtistHoverTip)
            .OrderBy(tip => tip is ArtistHoverTip ? 1 : 0)
            .ToList();

        if (!shiftHeld) return;

        foreach (var tip in list.OfType<ArtistHoverTip>())
        {
            var child = PreloadManager.Cache.GetScene("res://scenes/ui/hover_tip.tscn").Instantiate<Control>();
            __instance._textHoverTipContainer.AddChildSafely(child);

            var titleNode = child.GetNode<MegaLabel>("%Title");
            titleNode.SetTextAutoSize(tip.Title.GetFormattedText());

            child.GetNode<MegaRichTextLabel>("%Description").Text = "";
            child.GetNode<MegaRichTextLabel>("%Description").AutowrapMode = TextServer.AutowrapMode.WordSmart;
            child.GetNode<TextureRect>("%Icon").Texture = tip.Icon;

            var bg = child.GetNodeOrNull<CanvasItem>("Bg");
            if (bg != null)
            {
                var mat = new ShaderMaterial();
                mat.Shader = ResourceLoader.Load<Shader>("res://shaders/hsv.gdshader");
                mat.SetShaderParameter("h", 0.6f);
                mat.SetShaderParameter("s", 4.0f);
                mat.SetShaderParameter("v", 1.0f);
                bg.Material = mat;
            }

            child.ResetSize();
            if (NGame.Instance != null && __instance._textHoverTipContainer.Size.Y + child.Size.Y + 5.0 <
                NGame.Instance.GetViewportRect().Size.Y - 50.0)
                __instance._textHoverTipContainer.Size = new Vector2(360f,
                    __instance._textHoverTipContainer.Size.Y + child.Size.Y + 5.0f);
            else
                __instance._textHoverTipContainer.Alignment = FlowContainer.AlignmentMode.Center;
        }
    }
}