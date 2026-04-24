using Downfall.Code.Core.Champ;
using Downfall.Code.Core.Guardian;
using Downfall.Code.Core.Hexaghost;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Core;

public static class DownfallModelDb
{
    // Cached collections for iteration
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
    
    private static IEnumerable<GhostflameModel>? _allGhostflames;

    public static IEnumerable<GhostflameModel> AllGhostflames
    {
        get
        {
            if (_allGhostflames != null) return _allGhostflames;

            return _allGhostflames = ModelDb.AllAbstractModelSubtypes
                .Where(t => t.IsSubclassOf(typeof(GhostflameModel)))
                .Select(t => (GhostflameModel)ModelDb.Get(t))
                .ToList();
        }
    }

    public static T ChampStance<T>() where T : ChampStanceModel
    {
        return ModelDb.Get<T>();
    }

    public static T GuardianMode<T>() where T : GuardianModeModel
    {
        return ModelDb.Get<T>();
    }

    public static T Gem<T>() where T : GemModel
    {
        return ModelDb.Get<T>();
    }
    
    public static T Ghostflame<T>() where T : GhostflameModel
    {
        return ModelDb.Get<T>();
    }
}