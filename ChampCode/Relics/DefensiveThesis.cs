using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Events;
using Champ.ChampCode.Stance;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Champ.ChampCode.Relics;

[Pool(typeof(ChampRelicPool))]
public class DefensiveThesis() : ChampRelicModel(RelicRarity.Uncommon), IModifyFinisherBonus
{
    public int ModifyFinisherBonus(ChampStanceModel stanceModel, int baseAmount)
    {
        return stanceModel.Owner == Owner && stanceModel is ChampDefensiveStance ? baseAmount + 3 : baseAmount;
    }
}