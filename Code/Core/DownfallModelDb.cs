using Downfall.Code.Core.Champ;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Core;

public static class DownfallModelDb
{
    public static T ChampStance<T>() where T : StanceModel => ModelDb.GetById<T>(ModelDb.GetId<T>());
}