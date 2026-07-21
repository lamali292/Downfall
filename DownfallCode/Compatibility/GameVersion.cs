using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Downfall.DownfallCode.Compatibility;

public static class GameVersion
{
    /// <summary>New card-play API: CardLocation struct, Hook.ModifyCardPlayResultLocation, CardPlay.Player.</summary>
    public static readonly bool HasCardLocation =
        AccessTools.TypeByName("MegaCrit.Sts2.Core.Entities.Cards.CardLocation") != null;

    /// <summary>NCard portrait API rename.</summary>
    public static readonly bool HasNCardUpdatePortrait =
        AccessTools.Method(typeof(NCard), "UpdatePortrait") != null;
}