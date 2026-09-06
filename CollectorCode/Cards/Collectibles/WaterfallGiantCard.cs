using Collector.CollectorCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Rooms;

namespace Collector.CollectorCode.Cards.Collectibles;

public class WaterfallGiantCard()
    : Collectible<WaterfallGiantBoss>(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f)
{
}