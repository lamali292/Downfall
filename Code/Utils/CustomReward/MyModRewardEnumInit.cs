using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Downfall.Code;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;

[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.Init))]
class MyModRewardEnumInit
{
    [HarmonyPostfix]
    static void RegisterCustomRewards()
    {
        foreach (var t in ReflectionHelper.ModTypes)
        {
            foreach (var field in t.GetFields())
            {
                if (!Attribute.IsDefined(field, typeof(CustomEnumAttribute))) continue;
                if (field.FieldType != typeof(RewardType)) continue;
                if (!t.IsAssignableTo(typeof(CustomReward))) continue;

                var key = CustomEnums.GenerateKey(field);
                field.SetValue(null, key);

                if (t.CreateInstance() is not CustomReward dummyReward)
                {
                    DownfallMainFile.Logger.Error($"Failed to create reward instance for {t.FullName}");
                    continue;
                }

                dummyReward.Initialize();
            }
        }
    }
}