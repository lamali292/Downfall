using Collector.CollectorCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Encounters;

namespace Collector.CollectorCode.Cards.Collectibles;

public class SoulNexusCard()
    : Collectible<SoulNexusElite>(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 0.3f);
