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
    public IReadOnlyList<GemModel> Gems =>
        CardModifier.Modifiers(this).OfType<GemModel>().ToList();

    public void AddGem(GemModel gem)
    {
        if (IsFull) return;
        var mutableGem = gem.IsMutable ? gem : gem.ToMutable();
        CardModifier.AddModifier(this, mutableGem);
    }

    public void AddGems(IEnumerable<GemModel> gems)
    {
        foreach (var gem in gems)
        {
            if (IsFull) break;
            AddGem(gem);
        }
    }

    public bool CanAddGem(GemModel gem) => !IsFull;

    public int GemCount => Gems.Count;
    private bool IsFull => Gems.Count >= GemSlots;
    public int FreeSlots => Math.Max(0, GemSlots - Gems.Count);

    public virtual int GemSlots => 0;
    protected virtual int GemReplayCount => 1;

    protected GuardianCardModel(int cost, CardType type, CardRarity rarity, TargetType targetType,
        bool showInCardLibrary = true, bool autoAdd = true)
        : base(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
    {
        WithTips(card => card is GuardianCardModel gc ? gc.Gems.SelectMany(gem => gem.HoverTips) : []);
        if (this is ITickCard) WithTip(GuardianTip.Tick);
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