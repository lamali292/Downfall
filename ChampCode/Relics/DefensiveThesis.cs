using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Events;
using Champ.ChampCode.Stance;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;

namespace Champ.ChampCode.Relics;

[Pool(typeof(ChampRelicPool))]
public class DefensiveThesis : ChampRelicModel, IModifyDefensiveFinisherBonus
{
    public DefensiveThesis() : base(RelicRarity.Uncommon)
    {
        //WithTips(_ => ChampModelDb.ChampStance<ChampDefensiveStance>().HoverTips);
        WithTip(ChampTip.Finisher);
        WithBlock(3);
    }

    public int ModifyDefensiveFinisherBonus(ChampStanceModel stanceModel, int baseAmount)
    {
        return stanceModel.Owner == Owner ? baseAmount + DynamicVars.Block.IntValue : baseAmount;
    }
}