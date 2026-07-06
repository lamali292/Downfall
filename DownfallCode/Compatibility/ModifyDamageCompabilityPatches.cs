using System.Reflection;
using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.DownfallCode.Compatibility;


internal static class ModifyDamagePatchHelper
{
    public static MethodBase Find(string name)
    {
        const BindingFlags f = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        Type[] oldSig = [typeof(Creature), typeof(decimal), typeof(ValueProp),
            typeof(Creature), typeof(CardModel)];

        return typeof(AbstractModel).GetMethod(name, f, null, [.. oldSig, typeof(CardPlay)], null)
               ?? typeof(AbstractModel).GetMethod(name, f, null, oldSig, null)
               ?? throw new MissingMethodException($"{name} not found in any known signature.");
    }
}

public interface IModifyDamageAdditive
{
    decimal ModifyDamageAdditiveCompability(Creature? target, decimal amount,
        ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay) => 0;
}

public interface IModifyDamageMultiplicative
{
 decimal ModifyDamageMultiplicativeCompability(Creature? target, decimal amount,
        ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay) => 1;
}



[HarmonyPatch]
internal static class ModifyDamageAdditivePatch
{
    private static MethodBase TargetMethod() => ModifyDamagePatchHelper.Find("ModifyDamageAdditive");

    [HarmonyPostfix]
    private static void Postfix(AbstractModel __instance, object[] __args, ref decimal __result)
    {
        if (__instance is not IModifyDamageAdditive card) return;
        __result += card.ModifyDamageAdditiveCompability(
            (Creature?)__args[0], (decimal)__args[1], (ValueProp)__args[2],
            (Creature?)__args[3], (CardModel?)__args[4],
            __args.Length > 5 ? (CardPlay?)__args[5] : null);
    }
}

[HarmonyPatch]
internal static class ModifyDamageMultiplicativePatch
{
    private static MethodBase TargetMethod() => ModifyDamagePatchHelper.Find("ModifyDamageMultiplicative");

    [HarmonyPostfix]
    private static void Postfix(AbstractModel __instance, object[] __args, ref decimal __result)
    {
        if (__instance is not IModifyDamageMultiplicative card) return;
        __result *= card.ModifyDamageMultiplicativeCompability(
            (Creature?)__args[0], (decimal)__args[1], (ValueProp)__args[2],
            (Creature?)__args[3], (CardModel?)__args[4],
            __args.Length > 5 ? (CardPlay?)__args[5] : null);
    }
}

