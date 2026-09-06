using Collector.CollectorCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace Collector.CollectorCode.Cards.Collectibles;

public class KnowledgeDemonCard()
    : Collectible<KnowledgeDemonBoss>(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f);
