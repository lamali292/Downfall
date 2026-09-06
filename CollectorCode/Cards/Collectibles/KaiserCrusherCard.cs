using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace Collector.CollectorCode.Cards.Collectibles;

public class KaiserCrabCard() : Collectible<KaiserCrabBoss>(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f);