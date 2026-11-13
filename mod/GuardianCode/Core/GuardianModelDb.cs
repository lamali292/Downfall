using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Core;

public static class GuardianModelDb
{
    public static IEnumerable<GemModel> AllGems
    {
        get
        {
            if (field != null) return field;

            return field = ModelDb.AllAbstractModelSubtypes
                .Where(t => t.IsSubclassOf(typeof(GemModel)))
                .Select(t => ModelDb.GetById<GemModel>(ModelDb.GetId(t)))
                .ToList();
        }
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