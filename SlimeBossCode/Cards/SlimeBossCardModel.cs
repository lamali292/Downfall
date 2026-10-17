using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.DynamicVars;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards;

public abstract class SlimeBossCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool showInCardLibrary = true,
    bool autoAdd = true)
    : DownfallCardModel<Core.SlimeBoss>(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
{
   
    protected ConstructedCardModel WithSlimeTip<T>() where T : SlimeModel
    {
       return WithTips(_ =>
       {
           var slime = SlimeBossModelDb.Slime<T>();
           return new IHoverTip[] { slime.SlimeTip }.Concat(slime.ExtraTips);
       });
    }

    protected ConstructedCardModel WithCommand(decimal baseVal,
        decimal upgradedVal = 0)
    {
        WithVar(new CommandVar(baseVal).WithUpgrade(upgradedVal));
        return this;
    }
}