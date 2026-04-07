using System.Collections.Generic;
using System.Linq;
using Downfall.Code.Core;
using Downfall.Code.Core.Champ;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Downfall.Code.Events;

public static class DownfallSubscriber
{ 
    
    public static void Subscribe()
    {
        ModHelper.SubscribeForCombatStateHooks(DownfallMainFile.ModId, CollectModels2);
    }

    private static IEnumerable<AbstractModel> CollectModels2(CombatState combatState)
    {
        return combatState.Players
            .Select(ChampModel.GetStanceModel)
            .Where(s => s is not NoChampStance);
    }
    
}