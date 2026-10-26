using Collector.CollectorCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Collector.CollectorCode.Cards.Collectibles.Ruina2;

public abstract class Ruina2Card(int cost, CardType type, CardRarity rarity, TargetType targetType,
    string encounterId,
    float h = 0.0f, float s = 1.0f, float v = 1.0f) : CompatCollectible(cost, type, rarity, targetType, $"RUINA2-{encounterId}", "Ruina2", h, s, v);