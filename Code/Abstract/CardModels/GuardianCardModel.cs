using BaseLib.Utils;
using Downfall.Code.Core.Guardian;
using Downfall.Code.Extensions;
using Downfall.Code.Patches;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Abstract.CardModels;

public abstract class GuardianCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType)
    : DownfallCardModel<Character.Guardian>(cost, type, rarity, targetType), IAdditionalOverlay
{
    private static readonly SavedSpireField<CardModel, string?> _gemData = new(() => null, "DOWNFALL-GEM");
    private List<GemModel>? _cachedGems;

    public List<GemModel> Gems
    {
        get
        {
            _cachedGems ??= (_gemData.Get(this) ?? "")
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(ModelId.Deserialize)
                .Select(ModelDb.GetById<GemModel>)
                .ToList();
            return _cachedGems;
        }
    }
    
    public void AddGem(GemModel gem)
    {
        if (Gems.Count >= GemSlots) return;
        Gems.Add(gem);
        var newData = string.Join("|", Gems.Select(g => g.Id.ToString()));
        _gemData.Set(this, newData);
    }
    
    /*
    protected override void DeepCloneFields()
    {
        base.DeepCloneFields();
        if (_cachedGems != null)
        {
            _cachedGems = new List<GemModel>(_cachedGems);
        }
    }*/
    
    public static string GetRawGemData(CardModel card) => _gemData.Get(card)!;
    public static void SetRawGemData(CardModel card, string data) => _gemData.Set(card, data);
    
    protected virtual int GemSlots => 0;
    
    public bool IsFull => Gems.Count >= GemSlots;
    public int FreeSlots => Math.Max(0, GemSlots - Gems.Count);
    public bool CanAddGem(GemModel gem) => !IsFull;
    
    public Control CreateAdditionalOverlay()
    {
        var parentContainer = new Control
        {
            Name = OverlayNodeName,
            PivotOffset = new Vector2(150, 211),
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Position = new Vector2(-150, -211),
            Size = new Vector2(300, 422),
        };
        
        var vBox = new VBoxContainer
        {
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Position = new Vector2(240, 80),
        };
        parentContainer.AddChild(vBox);

        var currentGems = Gems; // Cache for performance
        for (var i = 0; i < GemSlots; i++)
        {
            var isFilled = i < currentGems.Count;
            var texturePath = isFilled ? currentGems[i].IconPath : "emptysocket.png".GemPath();
            var texture = PreloadManager.Cache.GetAsset<Texture2D>(texturePath);

            var rect = new TextureRect
            {
                Name = $"Slot_{i}",
                Texture = texture,
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                CustomMinimumSize = new Vector2(60f, 60f), 
                MouseFilter = Control.MouseFilterEnum.Ignore
            };
            vBox.AddChild(rect);
        }
        return parentContainer;
    }

    public string OverlayNodeName => "DownfallGemOverlay";

    protected virtual async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay) => await Task.CompletedTask;

    protected sealed override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PlayEffect(ctx, cardPlay);
        Gems.ForEach(g => g.OnPlay(ctx, cardPlay));
    }
}

[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.MutableClone))]
public static class MutableCloneGemsPatch
{
    [HarmonyPostfix]
    public static void Postfix(AbstractModel __instance, AbstractModel __result)
    {
        if (__instance is not GuardianCardModel parent || __result is not GuardianCardModel clone) return;
        var data = GuardianCardModel.GetRawGemData(parent);
        if (!string.IsNullOrEmpty(data))
        {
            GuardianCardModel.SetRawGemData(clone, data);
            
        }
    }
}
