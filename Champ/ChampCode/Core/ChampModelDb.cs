using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Core;

public class ChampModelDb
{
    public static T ChampStance<T>() where T : ChampStanceModel
    {
        return ModelDb.Get<T>();
    }

}