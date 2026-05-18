using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.DynamicVars;
using Guardian.GuardianCode.Interfaces;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Cards;

public abstract class GuardianCardModel : DownfallCardModel<Core.Guardian>
{
    public static readonly JsonSavedField<GuardianCardModel, List<SerializableGem>> GemData =
        JsonSavedField.Create<GuardianCardModel, List<SerializableGem>>("DOWNFALL_GEM");

    public List<GemModel>? CachedGems;

    protected GuardianCardModel(int cost, CardType type, CardRarity rarity, TargetType targetType)
        : base(cost, type, rarity, targetType)
    {
        WithTips(card => card is GuardianCardModel gc ? gc.Gems.SelectMany(gem => gem.HoverTips) : []);
        WithTips(card => card is GuardianCardModel gc ? gc.Gems.SelectMany(gem => gem.ExtraHoverTips) : []);
        if (GemSlots > 0)
        {
            WithTip(GuardianTip.Socket);
            WithTip(GuardianKeyword.Gem);
        }
        if (this is ITickCard)
        {
            WithTip(GuardianTip.Tick);
        }
    }

    public IReadOnlyList<GemModel> Gems
    {
        get
        {
            if (CachedGems != null) return CachedGems;
            CachedGems = (GemData.Get(this) ?? [])
                .Select(sg => sg.ToGem().ToMutable())
                .ToList();

            foreach (var gem in CachedGems) gem.Card = this;
            return CachedGems;
        }
    }

    public int GemCount => Gems.Count;
    public bool IsFull => Gems.Count >= GemSlots;
    public int FreeSlots => Math.Max(0, GemSlots - Gems.Count);

    public virtual int GemSlots => 0;
    protected virtual int GemReplayCount => 1;

    public void AddGem(GemModel gem)
    {
        if (IsFull) return;
        var mutableGem = gem.IsMutable ? gem : gem.ToMutable();
        CachedGems ??= Gems.ToList();
        CachedGems.Add(mutableGem);
        mutableGem.Card = this;
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
        var serializedGems = CachedGems?.Select(SerializableGem.FromGem).ToList();
        GemData.Set(this, serializedGems);
        //NCard.FindOnTable(this)?.ReloadOverlay();
        CardGemDisplay.Update(this);
    }

    protected ConstructedCardModel WithAccelerate(int baseVal, int upgradeVal = 0)
    {
        WithTip(GuardianTip.Accelerate, baseVal, upgradeVal);
        return WithVars(new AccelerateVar(baseVal).WithUpgrade(upgradeVal));
    }

    protected ConstructedCardModel WithBrace(int baseVal, int upgradeVal = 0)
    {
        WithTip(GuardianTip.Brace, baseVal, upgradeVal);
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
            await gem.OnPlayWrapper(ctx, cardPlay);
    }

    protected override void AfterCloned()
    {
        base.AfterCloned();
        if (CachedGems == null) return;
        var clonedGems = CachedGems.Select(gem => gem.CreateClone()).ToList();
        CachedGems = clonedGems;
        foreach (var gem in CachedGems)
            gem.Card = this;
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
        clone.CachedGems = null;
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