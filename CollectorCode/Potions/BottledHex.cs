using BaseLib.Utils;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Potions;

namespace Collector.CollectorCode.Potions;

[Pool(typeof(CollectorPotionPool))]
public class BottledHex() : CollectorPotionModel(PotionRarity.Common, PotionUsage.CombatOnly, TargetType.AnyEnemy)
{
    
}