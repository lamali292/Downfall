using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Core;

public static class GuardianModelDb
{
    private static IEnumerable<GemModel>? _allGems;
    public static IEnumerable<GemModel> AllGems
    {
        get
        {
            if (_allGems != null) return _allGems;

            return _allGems = ModelDb.AllAbstractModelSubtypes
                .Where(t => t.IsSubclassOf(typeof(GemModel)))
                .Select(t => (GemModel)ModelDb.Get(t))
                .ToList();
        }
    }
    
    public static T GuardianMode<T>() where T : GuardianModeModel
    {
        return ModelDb.Get<T>();
    }

    public static T Gem<T>() where T : GemModel
    {
        return ModelDb.Get<T>();
    }

}