using BaseLib.Patches.Content;
using HarmonyLib;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.HoverTips;

namespace Hexaghost.HexaghostCode.CustomEnums;

public static class HexaghostTip
{
    [CustomEnum] public static StaticHoverTip Ignite;
    [CustomEnum] public static StaticHoverTip Extinguish;
    [CustomEnum] public static StaticHoverTip Wheel;
}