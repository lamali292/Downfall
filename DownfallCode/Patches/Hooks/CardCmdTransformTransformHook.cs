using System.Reflection;
using Downfall.DownfallCode.Events;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch]
internal static class CardCmdTransformTransformHook
{
    private static MethodBase TargetMethod()
    {
        return typeof(CardCmd).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == nameof(CardCmd.Transform)
                        && m.GetParameters().Length == 3
                        && m.GetParameters()[1].ParameterType.Name == "Rng"
                        && m.GetParameters()[2].ParameterType.Name == "CardPreviewStyle");
    }

    private static void Postfix(ref Task<IEnumerable<CardPileAddResult>> __result)
    {
        var inner = __result;
        __result = FireHookAfter(inner);
    }

    private static async Task<IEnumerable<CardPileAddResult>> FireHookAfter(
        Task<IEnumerable<CardPileAddResult>> inner)
    {
        var results = await inner;
        var list = results as IList<CardPileAddResult> ?? results.ToList();

        foreach (var result in list)
        {
            if (!result.success) continue;
            var replacement = result.cardAdded;
            await MyHookUtils.Dispatch<IAfterCardTransformed>(
                replacement.CombatState,
                h => h.AfterCardTransformed(replacement),
                MyHookUtils.HookScope.Run,
                replacement.Owner.RunState);
        }

        return list;
    }
}