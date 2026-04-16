using Downfall.Code.Core.Champ;
using Downfall.Code.Core.Guardian;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Core;

public static class DownfallModelDb
{
    public static T ChampStance<T>() where T : ChampStanceModel
    {
        return ModelDb.GetById<T>(ModelDb.GetId<T>());
    }
    
    
    public static T GuardianMode<T>() where T : GuardianModeModel
    {
        return ModelDb.GetById<T>(ModelDb.GetId<T>());
    }
    
    
    public static T Gem<T>() where T : GemModel
    {
        return ModelDb.GetById<T>(ModelDb.GetId<T>());
    }
}