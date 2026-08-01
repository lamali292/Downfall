using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Guardian.GuardianCode.Cards;

public abstract class GuardianCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool showInCardLibrary = true,
    bool autoAdd = true)
    : DownfallCardModel<Core.Guardian>(cost, type, rarity, targetType, showInCardLibrary, autoAdd);