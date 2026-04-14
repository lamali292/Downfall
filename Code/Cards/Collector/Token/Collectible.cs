using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace Downfall.Code.Cards.Collector.Token;



[Pool(typeof(CollectibleCardPool))]
public abstract class ACollectible<T>() : Collectible<T>(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    where T : MonsterModel;

public interface ICollectible
{
    float HueShift => 0f;
    float Saturation => 1f;
    float Value => 1f;
    MonsterModel GetMonsterModel();
}

public abstract class Collectible<T>(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    float h = 0.0f,
    float s = 1.0f,
    float v = 1.0f) : CollectorCardModel(cost, type, rarity, targetType), ICollectible
    where T : MonsterModel
{
    public override bool HasBuiltInOverlay => true;
    
    public override List<(string, string)> Localization =>
    [
        ("title", GetMonsterModel().Title.GetFormattedText()),
        ("description", "")
    ];
    
    //public override string CustomPortraitPath => "collectible.png".CardImagePath<Character.Collector>();
    public override string CustomPortraitPath => "collectible.tres".CardImageAtlasPath<Character.Collector>();

    public MonsterModel GetMonsterModel()
    {
        return ModelDb.Monster<T>();
    }

    public float HueShift => h;
    public float Saturation => s;
    public float Value => v;
}

[HarmonyPatch(typeof(NCard), "Reload")]
public static class NCardReloadCollectiblePatch
{
    public static void Postfix(NCard __instance)
    {
        if (__instance.Model is not ICollectible collectible) return;

        var portrait = __instance.GetNodeOrNull<TextureRect>("%Portrait");
        if (portrait == null) return;

        var shaderMaterial = new ShaderMaterial();
        shaderMaterial.Shader = ResourceLoader.Load<Shader>("res://Downfall/shaders/hsv.gdshader");
        shaderMaterial.SetShaderParameter("h", collectible.HueShift);
        shaderMaterial.SetShaderParameter("s", collectible.Saturation);
        shaderMaterial.SetShaderParameter("v", collectible.Value);
        portrait.Material = shaderMaterial;
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.CreateOverlay))]
public static class CreateOverlayPatch
{
    public static bool Prefix(CardModel __instance, ref Control __result)
    {
        if (__instance is not ICollectible collectible) return true;

        try
        {
        var monsterModel = collectible.GetMonsterModel();
        var monster = monsterModel.ToMutable();
        var visuals = monster.CreateVisuals();

        var container = new Control
        {
            CustomMinimumSize = new Vector2(300f, 200f),
            ClipContents = false,
            Position = Vector2.Zero,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };

        container.AddChild(visuals);

        visuals.Ready += () =>
        {
            try
            {
                //monster.SetupSkins(visuals);
                if (visuals.SpineBody != null)
                    monster.GenerateAnimator(visuals.SpineBody);
                foreach (var node in visuals.GetChildrenRecursive<Control>())
                    node.MouseFilter = Control.MouseFilterEnum.Ignore;
                var boundsSize = visuals.Bounds.Size;
                var boundsPos = visuals.Bounds.Position;
                const float portraitW = 250f;
                const float portraitH = 190f;
                const float portraitCenterX = 0f;
                const float portraitBottom = 22f;

                const float fitScale = 0.8f;
                const float verticalPadding = (1.0f - fitScale) / 2.0f;

                var scale = Math.Min(portraitW / boundsSize.X, portraitH / boundsSize.Y) * fitScale;
                visuals.Scale = Vector2.One * scale;
                
                visuals.Position = new Vector2(
                    portraitCenterX - (boundsPos.X + boundsSize.X * 0.5f) * scale,
                    portraitBottom - boundsSize.Y * scale * verticalPadding
                );
            }
            catch (Exception e)
            {
                GD.PrintErr($"[Downfall] Failed to setup collectible visuals: {e.Message}");
            }
        };

        __result = container;
        return false;
        }
        catch (Exception e)
        {
            GD.PrintErr($"[Downfall] Failed to create collectible overlay: {e.Message}");
            return true; // fall back to default overlay
        }
    }
}