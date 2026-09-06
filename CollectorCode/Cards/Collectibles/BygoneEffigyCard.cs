using Collector.CollectorCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Rooms;

namespace Collector.CollectorCode.Cards.Collectibles;

public class BygoneEffigyCard()
    : Collectible<BygoneEffigyElite>(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 0.3f)
{
}
