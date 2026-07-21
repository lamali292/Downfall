using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Events;
using Champ.ChampCode.Stance;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Champ.ChampCode.Relics;

[Pool(typeof(ChampRelicPool))]
public class DefensiveThesis : ChampRelicModel, IModifyDefensiveFinisherBonus
{
    public DefensiveThesis() : base(RelicRarity.Uncommon)
    {
        WithTips(_ => [ChampModelDb.ChampStance<ChampDefensiveStance>().HoverTip]);
        WithTip(ChampTip.Finisher);
    }

    public int ModifyDefensiveFinisherBonus(ChampStanceModel stanceModel, int baseAmount)
    {
        return stanceModel.Owner == Owner ? baseAmount + 3 : baseAmount;
    }
}