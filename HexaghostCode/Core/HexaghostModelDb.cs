using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Core;

public static class HexaghostModelDb
{
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

    public static T Ghostflame<T>() where T : GhostflameModel
    {
        return ModelDb.Get<T>();
    }
}