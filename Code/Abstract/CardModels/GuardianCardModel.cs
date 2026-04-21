using Downfall.Code.Character;
using Downfall.Code.Core.Guardian;
using Downfall.Code.Extensions;
using Downfall.Code.Patches;
using Downfall.Code.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Downfall.Code.Abstract.CardModels;

public abstract class GuardianCardModel
    : DownfallCardModel<Guardian>, IAdditionalOverlay
{
    public static readonly JsonSavedField<CardModel, List<SerializableGem>> GemData =
        JsonSavedField.Create<CardModel, List<SerializableGem>>("DOWNFALL_GEM");

    private List<GemModel>? _cachedGems;

    protected GuardianCardModel(
        int cost,
        CardType type,
        CardRarity rarity,
        TargetType targetType) : base(cost, type, rarity, targetType)
    {
        // TODO : Wait for BaseLib to support this
        // WithTips(card => card is GuardianCardModel gc ? gc.Gems.SelectMany(gem => gem.ExtraHoverTips) : []);
    }

    public IReadOnlyList<GemModel> Gems
    {
        get
        {
            _cachedGems ??= (GemData.Get(this) ?? [])
                .Select(sg => sg.ToGem())
                .ToList();
            return _cachedGems;
        }
    }

    public virtual int GemSlots => 0;

    public bool IsFull => Gems.Count >= GemSlots;
    public int FreeSlots => Math.Max(0, GemSlots - Gems.Count);

    public Control CreateAdditionalOverlay()
    {
        var parentContainer = new Control
        {
            Name = OverlayNodeName,
            PivotOffset = new Vector2(150, 211),
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Position = new Vector2(-150, -211),
            Size = new Vector2(300, 422)
        };

        var vBox = new VBoxContainer
        {
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Position = new Vector2(240, 80)
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

    private void UpdateGemData()
    {
        var serializedGems = _cachedGems?
            .Select(SerializableGem.FromGem)
            .ToList();

        GemData.Set(this, serializedGems);
        NCard.FindOnTable(this)?.ReloadOverlay();
    }

    public void AddGem(GemModel gem)
    {
        if (Gems.Count >= GemSlots) return;
        _cachedGems ??= Gems.ToList();
        _cachedGems.Add(gem);
        UpdateGemData();
    }

    public void AddGems(IEnumerable<GemModel> gems)
    {
        _cachedGems ??= Gems.ToList();

        foreach (var gem in gems)
        {
            if (_cachedGems.Count >= GemSlots) break;
            _cachedGems.Add(gem);
        }

        NCard.FindOnTable(this)?.ReloadOverlay();
        UpdateGemData();
    }

    public void RemoveGem(GemModel gem)
    {
        _cachedGems ??= Gems.ToList();
        _cachedGems.Remove(gem);
        UpdateGemData();
    }

    public void ClearGems()
    {
        _cachedGems = [];
        UpdateGemData();
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

    public static List<SerializableGem>? GetRawGemData(CardModel card)
    {
        return GemData.Get(card);
    }

    public static void SetRawGemData(CardModel card, List<SerializableGem>? data)
    {
        GemData.Set(card, data);
    }

    public bool CanAddGem(GemModel gem)
    {
        return !IsFull;
    }

    protected virtual async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await Task.CompletedTask;
    }

    protected sealed override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PlayEffect(ctx, cardPlay);
        foreach (var gem in Gems) await gem.OnPlay(ctx, cardPlay);
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
        if (data == null) return;
        GuardianCardModel.SetRawGemData(clone, data);
    }
}