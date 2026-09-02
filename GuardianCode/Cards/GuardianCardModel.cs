using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.Abstract;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.DynamicVars;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Guardian.GuardianCode.Cards;

public abstract class GuardianCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool showInCardLibrary = true,
    bool autoAdd = true)
    : DownfallCardModel<Core.Guardian>(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
{
    public ConstructedCardModel WithAccelerate(int baseVal, int upgradeVal = 0)
    {
        WithTip(GuardianTip.Accelerate, baseVal, upgradeVal);
        return WithVars(new AccelerateVar(baseVal).WithUpgrade(upgradeVal));
    }

    public ConstructedCardModel WithBrace(int baseVal, int upgradeVal = 0)
    {
        WithTip(GuardianTip.Brace, baseVal, upgradeVal);
        return WithVars(new BraceVar(baseVal).WithUpgrade(upgradeVal));
    }

    public ConstructedCardModel WithPolish(int baseVal, int upgradeVal = 0)
    {
        WithTip(GuardianTip.Polish);
        return WithVars(new PolishVar(baseVal).WithUpgrade(upgradeVal));
    }
}