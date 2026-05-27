using Collector.CollectorCode.Interfaces;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Collector.CollectorCode.Cards;

public abstract class CollectorCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool showInCardLibrary = true,
    bool autoAdd = true)
    : DownfallCardModel<Core.Collector>(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
{
    protected override bool IsPlayable =>
        this is not IHasPyre || (this is IHasPyre && Owner.GetHand().Any(e => e != this));
}