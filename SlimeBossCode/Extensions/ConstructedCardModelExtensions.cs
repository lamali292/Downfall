using BaseLib.Abstracts;
using BaseLib.Extensions;
using SlimeBoss.SlimeBossCode.DynamicVars;

namespace SlimeBoss.SlimeBossCode.Extensions;

public static class ConstructedCardModelExtensions
{
    extension(ConstructedCardModel card)
    {
        public ConstructedCardModel WithSlurp(decimal baseVal,
            decimal upgradedVal = 0)
        {
            card.WithVar(new SlurpVar(baseVal).WithUpgrade(upgradedVal));
            return card;
        }

        public ConstructedCardModel WithCommand(decimal baseVal,
            decimal upgradedVal = 0)
        {
            card.WithVar(new CommandVar(baseVal).WithUpgrade(upgradedVal));
            return card;
        }
    }
    

}