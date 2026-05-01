using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Patches;
using Downfall.DownfallCode.Utils;
using Godot;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.DynamicVars;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Guardian.GuardianCode.Cards;

public abstract class GuardianCardModel : DownfallCardModel<Core.Guardian>, IAdditionalOverlay
{
    public static readonly JsonSavedField<GuardianCardModel, List<SerializableGem>> GemData =
        JsonSavedField.Create<GuardianCardModel, List<SerializableGem>>("DOWNFALL_GEM");

    public List<GemModel>? _cachedGems;

    protected GuardianCardModel(int cost, CardType type, CardRarity rarity, TargetType targetType)
        : base(cost, type, rarity, targetType)
    {
        WithTips(card => card is GuardianCardModel gc ? gc.Gems.SelectMany(gem => gem.HoverTips) : []);
        WithTips(card => card is GuardianCardModel gc ? gc.Gems.SelectMany(gem => gem.ExtraHoverTips) : []);
    }

    public IReadOnlyList<GemModel> Gems
    {
        get
        {
            if (_cachedGems != null) return _cachedGems;
            _cachedGems = (GemData.Get(this) ?? [])
                .Select(sg => sg.ToGem().ToMutable())
                .ToList();

            foreach (var gem in _cachedGems) gem.SetCard(this);
            return _cachedGems;
        }
    }

    public bool IsFull => Gems.Count >= GemSlots;
    public int FreeSlots => Math.Max(0, GemSlots - Gems.Count);

    public virtual int GemSlots => 0;
    protected virtual int GemReplayCount => 1;

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

        var currentGems = Gems;
        for (var i = 0; i < GemSlots; i++)
        {
            var isFilled = i < currentGems.Count;
            var texture = isFilled ? currentGems[i].Icon : GemModel.EmptyIcon;

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

    public void AddGem(GemModel gem)
    {
        if (IsFull) return;
        var mutableGem = gem.IsMutable ? gem : gem.ToMutable();
        _cachedGems ??= Gems.ToList();
        _cachedGems.Add(mutableGem);
        mutableGem.SetCard(this);
        UpdateGemData();
    }

    public void AddGems(IEnumerable<GemModel> gems)
    {
        foreach (var gem in gems)
        {
            if (IsFull) break;
            AddGem(gem);
        }
    }

    public bool CanAddGem(GemModel gem)
    {
        return !IsFull;
    }

    private void UpdateGemData()
    {
        var serializedGems = _cachedGems?.Select(SerializableGem.FromGem).ToList();
        GemData.Set(this, serializedGems);
        NCard.FindOnTable(this)?.ReloadOverlay();
    }

    protected ConstructedCardModel WithAccelerate(int baseVal, int upgradeVal = 0)
    {
        WithTip(GuardianTip.Accelerate);
        return WithVars(new AccelerateVar(baseVal).WithUpgrade(upgradeVal));
    }

    protected ConstructedCardModel WithBrace(int baseVal, int upgradeVal = 0)
    {
        WithTip(GuardianTip.Brace);
        return WithVars(new BraceVar(baseVal).WithUpgrade(upgradeVal));
    }

    protected ConstructedCardModel WithPolish(int baseVal, int upgradeVal = 0)
    {
        WithTip(GuardianTip.Polish);
        return WithVars(new PolishVar(baseVal).WithUpgrade(upgradeVal));
    }

    protected virtual async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await Task.CompletedTask;
    }

    protected sealed override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PlayEffect(ctx, cardPlay);
        foreach (var gem in Gems.SelectMany(gem => Enumerable.Repeat(gem, GemReplayCount)))
            await gem.OnPlay(ctx, cardPlay);
    }

    protected override void AfterCloned()
    {
        base.AfterCloned();
        if (_cachedGems == null) return;
        var clonedGems = _cachedGems.Select(gem => gem.CreateClone()).ToList();
        _cachedGems = clonedGems;
        foreach (var gem in _cachedGems)
            gem.SetCard(this);
    }
}

[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.MutableClone))]
public static class MutableCloneGemsPatch
{
    [HarmonyPostfix]
    public static void Postfix(AbstractModel __instance, AbstractModel __result)
    {
        if (__instance is not GuardianCardModel parent || __result is not GuardianCardModel clone) return;

        var data = GuardianCardModel.GemData.Get(parent);
        if (data == null) return;
        GuardianCardModel.GemData.Set(clone, data);

        // Clear cached gems so they reload with correct card references
        clone._cachedGems = null;
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.GetEnchantedReplayCount))]
internal static class GetEnchantedReplayCountPatch
{
    [HarmonyPostfix]
    private static void AddGemReplayCount(CardModel __instance, ref int __result)
    {
        if (__instance is not GuardianCardModel guardianCard) return;
        __result = guardianCard.Gems.Aggregate(__result, (current, gem) => gem.ModifyPlayCount(current));
    }
}