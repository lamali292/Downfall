using Downfall.DownfallCode.Extensions;
using Xunit;

namespace Downfall.UnitTests;

public class CardCostRulesTests
{
    [Theory]
    [InlineData(false, false, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(true, true, false)]
    public void HasNumericCost_OnlyWhenNeitherXEnergyNorXStar(bool xEnergy, bool xStar, bool expected)
    {
        Assert.Equal(expected, CardCostRules.HasNumericCost(xEnergy, xStar));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(9)]
    public void EffectiveCost_XEnergyPaysAvailableEnergy(int energy)
    {
        Assert.Equal(energy, CardCostRules.EffectiveCost(true, energy, 0));
        Assert.Equal(energy, CardCostRules.EffectiveCost(true, energy, 5));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(4)]
    public void EffectiveCost_NormalCardPaysModifiedCost(int cost)
    {
        Assert.Equal(cost, CardCostRules.EffectiveCost(false, 3, cost));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-2)]
    public void EffectiveCost_NegativeModifiedCostClampsToZero(int cost)
    {
        Assert.Equal(0, CardCostRules.EffectiveCost(false, 3, cost));
    }
}
