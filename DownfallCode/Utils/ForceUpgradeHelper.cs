using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Utils;

public static class ForceUpgradeHelper
{
    internal static readonly ConditionalWeakTable<CardModel, StrongBox<int>> ForceUpgraded = new();

    public static void ForceUpgrade(CardModel card, int times = 1)
    {
        var box = ForceUpgraded.GetOrCreateValue(card);
        for (var i = 0; i < times; i++)
        {
            box.Value = card._currentUpgradeLevel + 1;
            card.UpgradeInternal();
            card.FinalizeUpgradeInternal();
            box.Value = card._currentUpgradeLevel;
        }
    }
}