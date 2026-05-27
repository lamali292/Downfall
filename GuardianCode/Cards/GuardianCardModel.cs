using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.Abstract;
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
    protected GuardianCardModel(int cost, CardType type, CardRarity rarity, TargetType targetType,
        bool showInCardLibrary = true, bool autoAdd = true)
        : base(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
    {
        WithTips(card => card is IGemSocketCard gc ? gc.Gems.SelectMany(gem => gem.HoverTips) : []);
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
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.GetEnchantedReplayCount))]
internal static class GetEnchantedReplayCountPatch
{
    [HarmonyPostfix]
    private static void AddGemReplayCount(CardModel __instance, ref int __result)
    {
        if (__instance is not IGemSocketCard guardianCard) return;
        __result = guardianCard.Gems.Aggregate(__result, (current, gem) => gem.ModifyPlayCount(current));
    }
}