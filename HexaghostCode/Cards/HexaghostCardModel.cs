using BaseLib.Abstracts;
using Downfall.DownfallCode.Abstract;
using Hexaghost.HexaghostCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Hexaghost.HexaghostCode.Cards;

public abstract class HexaghostCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool showInCardLibrary = true,
    bool autoAdd = true)
    : DownfallCardModel<Core.Hexaghost>(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
{
    public ConstructedCardModel WithAfterlife()
    {
        WithKeywords(CardKeyword.Ethereal, HexaghostKeyword.Afterlife);
        return this;
    }
}