using Downfall.DownfallCode.Abstract;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Guardian.GuardianCode.Cards;

public abstract class GuardianCardModel : DownfallCardModel<Core.Guardian>
{
    protected GuardianCardModel(int cost, CardType type, CardRarity rarity, TargetType targetType,
        bool showInCardLibrary = true, bool autoAdd = true)
        : base(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
    {
        WithTips(card => card is IGemSocketCard gc ? gc.Gems.SelectMany(gem => gem.HoverTips) : []);
        if (this is ITickCard) WithTip(GuardianTip.Tick);
    }
}