using BaseLib.Utils;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Commands;

public static class MyCommonActions
{
    public static async Task<T?> ApplySelf<T>(CardModel card, bool silent = false) where T : PowerModel
    {
        return await CommonActions.ApplySelf<T>(card, card.DynamicVars.Power<T>().BaseValue, silent);
    }

    public static async Task<T?> Apply<T>(
        Creature target,
        CardModel card,
        bool silent = false)
        where T : PowerModel
    {
        return await CommonActions.Apply<T>(target, card, card.DynamicVars.Power<T>().BaseValue, silent);
    }
}