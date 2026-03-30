using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Collector.Common;

[Pool(typeof(CollectorCardPool))]
public class Torchbearer() : CollectorCardModel(2, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // TODO: Implement
}