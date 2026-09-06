using Collector.CollectorCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Encounters;

namespace Collector.CollectorCode.Cards.Collectibles;

public class AeonglassCard()
    : Collectible<AeonglassBoss>(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f)
{
    
}