using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Events;
using Champ.ChampCode.Stance;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Champ.ChampCode.Relics;

[Pool(typeof(ChampRelicPool))]
public class DefensiveThesis : ChampRelicModel, IModifyFinisherBonus
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    
    public int ModifyFinisherBonus(ChampStanceModel stanceModel, int baseAmount)
    {
       return stanceModel.Owner == Owner && stanceModel is ChampDefensiveStance ?  baseAmount + 3 : baseAmount;
    }
}