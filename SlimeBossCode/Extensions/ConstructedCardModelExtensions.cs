using BaseLib.Abstracts;
using BaseLib.Extensions;
using SlimeBoss.SlimeBossCode.DynamicVars;

namespace SlimeBoss.SlimeBossCode.Extensions;

public static class ConstructedCardModelExtensions
{
    public static ConstructedCardModel WithSlurp(this ConstructedCardModel card, decimal baseVal,
        decimal upgradedVal = 0)
    {
        card.WithVar(new SlurpVar(baseVal).WithUpgrade(upgradedVal));
        return card;
    }

    public static ConstructedCardModel WithCommand(this ConstructedCardModel card, decimal baseVal,
        decimal upgradedVal = 0)
    {
        card.WithVar(new CommandVar(baseVal).WithUpgrade(upgradedVal));
        return card;
    }
}