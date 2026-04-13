using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Collector.Basic;

[Pool(typeof(CollectorCardPool))]
public class DefendCollector() : CollectorCardModel(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    // TODO: Implement
}