using BaseLib.Abstracts;
using Downfall.DownfallCode.Patches;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace Collector.CollectorCode.Cards.Token;

public interface ICollectible
{
    MonsterModel GetMonsterModel();
}

public abstract class Collectible<T>(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    float h = 0.0f,
    float s = 1.0f,
    float v = 1.0f) : CollectorCardModel(cost, type, rarity, targetType), ICollectible, IAdditionalOverlay,
    IColoredPortrait
    where T : MonsterModel
{
    public override bool HasBuiltInOverlay => false;

    public override List<(string, string)> Localization =>
        new CardLoc(
            GetMonsterModel().Title.GetFormattedText(),
            ""
        );


    //public override string CustomPortraitPath => "collectible.png".CardImagePath<Character.Collector>();
    public override string CustomPortraitPath => "collectible.tres".CardImageAtlasPath<Core.Collector>();


    public Control CreateAdditionalOverlay()
    {
        var monster = GetMonsterModel().ToMutable();
        var visuals = monster.CreateVisuals();

        var container = new Control { Name = OverlayNodeName, MouseFilter = Control.MouseFilterEnum.Ignore };
        container.AddChild(visuals);

        visuals.Ready += () => S(visuals, monster);

        return container;
    }

    public string OverlayNodeName => "DownfallMonsterOverlay";

    public MonsterModel GetMonsterModel()
    {
        return ModelDb.Monster<T>();
    }

    public float HueShift => h;
    public float Saturation => s;
    public float Value => v;

    public void S(NCreatureVisuals visuals, MonsterModel monster)
    {
        if (visuals.SpineBody != null)
            monster.GenerateAnimator(visuals.SpineBody);

        foreach (var node in visuals.GetChildrenRecursive<Control>())
            node.MouseFilter = Control.MouseFilterEnum.Ignore;

        // --- YOUR CUSTOM VALUES ---
        var boundsSize = visuals.Bounds.Size;
        var boundsPos = visuals.Bounds.Position;
        const float portraitW = 250f;
        const float portraitH = 190f;
        const float portraitCenterX = 0f;
        const float portraitBottom = 0; //22f;
        const float fitScale = 0.6f;
        const float verticalPadding = (1.0f - fitScale) / 2.0f;

        var scale = Math.Min(portraitW / boundsSize.X, portraitH / boundsSize.Y) * fitScale;
        visuals.Scale = Vector2.One * scale;

        visuals.Position = new Vector2(
            portraitCenterX - (boundsPos.X + boundsSize.X * 0.5f) * scale,
            portraitBottom - boundsSize.Y * scale * verticalPadding
        );
    }
}