using BaseLib.Patches.Content;
using BaseLib.Patches.Features;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.CustomEnums;

public class DownfallTargetType
{
    [CustomEnum]
    public static TargetType MeAndEnemies;
}

[HarmonyPatch(typeof (ModelDb), "Init")]
public static class ModelDbTargetTypeInitPatch
{
  [HarmonyPostfix]
  public static void RegisterTargetTypes()
  {
    CustomTargetType.RegisterMultiTargetType(DownfallTargetType.MeAndEnemies,  (target, player) => target is { IsAlive: true, IsPet: false, IsEnemy: true } || target == player.Creature);
  }
}
