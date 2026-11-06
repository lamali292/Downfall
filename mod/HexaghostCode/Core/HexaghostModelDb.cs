using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Core;

public static class HexaghostModelDb
{
    public static IEnumerable<GhostflameModel> AllGhostflames
    {
        get
        {
            if (field != null) return field;

            return field = ModelDb.AllAbstractModelSubtypes
                .Where(t => t.IsSubclassOf(typeof(GhostflameModel)))
                .Select(t => ModelDb.GetById<GhostflameModel>(ModelDb.GetId(t)))
                .ToList();
        }
    }

    public static T Ghostflame<T>() where T : GhostflameModel
    {
        return ModelDb.GetById<T>(ModelDb.GetId<T>());
    }
}