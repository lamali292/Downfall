using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.DownfallCode.Compatibility;

public interface IModifyDamageAdditive
{
    decimal ModifyDamageAdditiveCompability(Creature? target, decimal amount,
        ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay);
}

public interface IModifyDamageMultiplicative
{
    decimal ModifyDamageMultiplicativeCompability(Creature? target, decimal amount,
        ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay);
}

public interface IModifyCardPlayResultLocation
{
    CardLocationCompatiblity ModifyCardPlayResultLocationCompability(CardModel card, bool isAutoPlay, ResourceInfo resources,
        CardLocationCompatiblity cardLocation) => cardLocation;

    Task AfterModifyingCardPlayResultLocationCompability(CardModel card, CardLocationCompatiblity cardLocation) => Task.CompletedTask;
}

public record struct CardLocationCompatiblity(Player Player, PileType PileType, CardPilePosition Position);
