using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.Abstract;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.DynamicVars;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Cards;

public abstract class GuardianCardModel : DownfallCardModel<Core.Guardian>
{
    protected GuardianCardModel(
        int cost,
        CardType type,
        CardRarity rarity,
        TargetType targetType,
        bool showInCardLibrary = true,
        bool autoAdd = true)
        : base(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
    {
        WithTips(EmptySocketTip);
    }

    // Socket cards explain "Socket" only while they still have an empty one to fill.
    private static IEnumerable<IHoverTip> EmptySocketTip(CardModel card)
    {
        if (card is IGemSocketCard { FreeSlots: > 0 }) yield return HoverTipFactory.Static(GuardianTip.Socket);
    }

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