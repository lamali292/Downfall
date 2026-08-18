using BaseLib.Abstracts;
using BaseLib.Extensions;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.DynamicVars;

namespace Guardian.GuardianCode.Extensions;

public static class ConstructedCardModelExtensions
{
    extension(ConstructedCardModel card)
    {
        public ConstructedCardModel WithAccelerate(int baseVal, int upgradeVal = 0)
        {
            card.WithTip(GuardianTip.Accelerate, baseVal, upgradeVal);
            return card.WithVars(new AccelerateVar(baseVal).WithUpgrade(upgradeVal));
        }

        public ConstructedCardModel WithBrace(int baseVal, int upgradeVal = 0)
        {
            card.WithTip(GuardianTip.Brace, baseVal, upgradeVal);
            return card.WithVars(new BraceVar(baseVal).WithUpgrade(upgradeVal));
        }

        public ConstructedCardModel WithPolish(int baseVal, int upgradeVal = 0)
        {
            card.WithTip(GuardianTip.Polish);
            return card.WithVars(new PolishVar(baseVal).WithUpgrade(upgradeVal));
        }
    }
    
   
}