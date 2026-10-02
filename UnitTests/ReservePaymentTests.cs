using Collector.CollectorCode.Core;
using Xunit;

namespace Downfall.UnitTests;

public class ReservePaymentTests
{
    [Theory]
    [InlineData(0, 2, 2, true, true)]
    [InlineData(3, 1, 2, true, false)] // Energy does not help a reserve-only card
    [InlineData(1, 1, 2, false, true)]
    [InlineData(1, 0, 2, false, false)]
    [InlineData(3, 0, 3, false, true)]
    public void CanAfford(int energy, int reserve, int cost, bool reserveOnly, bool expected)
    {
        Assert.Equal(expected, ReservePaymentRules.CanAfford(energy, reserve, cost, reserveOnly));
    }

    [Fact]
    public void ReserveOnly_PaysFullCostFromReserve()
    {
        Assert.Equal(new ReservePayment(3, false), ReservePaymentRules.Pay(2, 5, 3, false, true));
    }

    [Fact]
    public void ReserveOnly_InsufficientPaysNothing()
    {
        Assert.Equal(new ReservePayment(0, false), ReservePaymentRules.Pay(9, 2, 3, false, true));
    }

    [Fact]
    public void Mixed_EnoughEnergyLeavesReserveAlone()
    {
        Assert.Equal(new ReservePayment(0, false), ReservePaymentRules.Pay(3, 5, 2, false, false));
    }

    [Fact]
    public void Mixed_ReserveCoversOnlyTheDeficit()
    {
        Assert.Equal(new ReservePayment(1, false), ReservePaymentRules.Pay(1, 5, 2, false, false));
    }

    [Fact]
    public void Mixed_ReserveCapsAtWhatIsAvailable()
    {
        Assert.Equal(new ReservePayment(1, false), ReservePaymentRules.Pay(0, 1, 3, false, false));
    }

    [Fact]
    public void Mixed_ZeroCostPaysNothing()
    {
        Assert.Equal(new ReservePayment(0, false), ReservePaymentRules.Pay(0, 4, 0, false, false));
    }

    [Fact]
    public void XCost_ConvertsAllReserveToEnergy()
    {
        Assert.Equal(new ReservePayment(3, true), ReservePaymentRules.Pay(2, 3, 0, true, false));
    }

    [Fact]
    public void XCost_WithNoReserveConvertsNothing()
    {
        Assert.Equal(new ReservePayment(0, true), ReservePaymentRules.Pay(4, 0, 0, true, false));
    }

    [Fact]
    public void XCost_TakesPrecedenceOverReserveOnly()
    {
        Assert.Equal(new ReservePayment(2, true), ReservePaymentRules.Pay(0, 2, 0, true, true));
    }
}
