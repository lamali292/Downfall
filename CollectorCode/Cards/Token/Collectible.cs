using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Interfaces;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves;

namespace Collector.CollectorCode.Cards.Token;

public interface ICollectible
{
    EncounterModel? GetEncounterModel();
    ActModel? Act();
    RoomType? RoomType();
}

public abstract class CompatCollectible : Collectible
{
    private readonly ModelId _encounterModelId;

    protected CompatCollectible(int cost, CardType type, CardRarity rarity, TargetType targetType,
        string encounterId, string requiredModId,
        float h = 0.0f, float s = 1.0f, float v = 1.0f)
        : base(cost, type, rarity, targetType, h, s, v, IsModLoaded(requiredModId), IsModLoaded(requiredModId))
    {
        if (string.IsNullOrEmpty(encounterId))
            throw new ArgumentException("encounterId must not be null or empty.", nameof(encounterId));

        _encounterModelId = new ModelId("ENCOUNTER", encounterId);
    }
    private static bool IsModLoaded(string modId)
    {
        return ModManager.GetLoadedMods().Any(m => m.manifest?.id == modId);
    }
    public override EncounterModel? GetEncounterModel() => ModelDb.GetByIdOrNull<EncounterModel>(_encounterModelId);
}


public abstract class Collectible<T>(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    float h = 0.0f,
    float s = 1.0f,
    float v = 1.0f) : Collectible(cost, type, rarity, targetType, h, s, v)
    where T : EncounterModel
{
    public override EncounterModel GetEncounterModel()
    {
        return ModelDb.Encounter<T>();
    }
}

[Pool(typeof(CollectibleCardPool))]
public abstract class Collectible(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    float h = 0.0f,
    float s = 1.0f,
    float v = 1.0f,
    bool showInCardLibrary = true,
    bool autoAdd = true) : CollectorCardModel(cost, type, rarity, targetType, showInCardLibrary, autoAdd), ICollectible, IAdditionalOverlay,
    IColoredPortrait
{
    public override bool HasBuiltInOverlay => false;
   
    public ActModel? Act() => ModelDb.Acts.FirstOrDefault(e => e.AllEncounters.Contains(EncounterModel));
    public RoomType? RoomType() => EncounterModel?.RoomType;
    
    //public override string CustomPortraitPath => "collectible.png".CardImagePath<Character.Collector>();
    public override string CustomPortraitPath => "collectible.tres".CardImageAtlasPath<Core.Collector>();


    public Control CreateAdditionalOverlay()
    {
        var monster = EncounterModel?.AllPossibleMonsters.FirstOrDefault()?.ToMutable();
        var container = new Control { Name = OverlayNodeName, MouseFilter = Control.MouseFilterEnum.Ignore };
        if (monster ==  null) return container;
        var visuals = monster.CreateVisuals();
        container.AddChild(visuals);

        visuals.Ready += () => S(visuals, monster);

        return container;
    }

    public string OverlayNodeName => "DownfallMonsterOverlay";

    public abstract EncounterModel? GetEncounterModel();
    private EncounterModel? EncounterModel => GetEncounterModel();

    public float HueShift => h;
    public float Saturation => s;
    public float Value => v;

    private static void S(NCreatureVisuals visuals, MonsterModel monster)
    {
        if (visuals.SpineBody != null)
        {
            monster.GenerateAnimator(visuals.SpineBody);
            var skeleton = visuals.SpineBody.GetSkeleton();
            if (skeleton == null)
                return;
            try
            {
                _ = new Creature(monster, CombatSide.Enemy, "hi");
                monster.SetupSkins(visuals.SpineBody, skeleton);
                monster.OnPhobiaModeToggled(SaveManager.Instance.PrefsSave.PhobiaMode, visuals.SpineBody, skeleton);
                //visuals.SpineAnimation.SetAnimation("attack", true);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e);
                throw;
            }
          
        }
        
        foreach (var node in visuals.GetChildrenRecursive<Control>())
            node.MouseFilter = Control.MouseFilterEnum.Ignore;
        
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