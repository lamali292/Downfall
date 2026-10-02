using Hermit.HermitCode.Core;
using Xunit;

namespace Downfall.UnitTests;

public class HandGeometryTests
{
    private static int[] Centers(int size) =>
        Enumerable.Range(-2, size + 4).Where(i => HandGeometry.IsCenter(i, size)).ToArray();

    [Theory]
    [InlineData(0, new int[0])]
    [InlineData(1, new[] { 0 })]
    [InlineData(2, new[] { 0, 1 })]
    [InlineData(3, new[] { 1 })]
    [InlineData(4, new[] { 1, 2 })]
    [InlineData(5, new[] { 2 })]
    [InlineData(6, new[] { 2, 3 })]
    [InlineData(7, new[] { 3 })]
    [InlineData(8, new[] { 3, 4 })]
    [InlineData(9, new[] { 4 })]
    [InlineData(10, new[] { 4, 5 })]
    public void IsCenter_Index_MatchesMiddleIndices(int size, int[] expected)
    {
        // Centers() also probes indices below 0 and past the end, so those must be false too.
        Assert.Equal(expected, Centers(size));
    }

    [Fact]
    public void IsCenter_Item_FindsMiddleOfOddHand()
    {
        var hand = new[] { "a", "b", "c" };
        Assert.True(HandGeometry.IsCenter(hand, "b"));
        Assert.False(HandGeometry.IsCenter(hand, "a"));
        Assert.False(HandGeometry.IsCenter(hand, "c"));
    }

    [Fact]
    public void IsCenter_Item_AbsentItemIsNotCenter()
    {
        Assert.False(HandGeometry.IsCenter(new[] { "a", "b", "c" }, "z"));
        Assert.False(HandGeometry.IsCenter(Array.Empty<string>(), "a"));
    }

    [Fact]
    public void IsCenter_Item_DuplicatesUseFirstOccurrence()
    {
        // "x" first appears at index 0 (not center), even though a later copy sits at the center.
        var hand = new[] { "x", "b", "x" };
        Assert.False(HandGeometry.IsCenter(hand, "x"));

        // First occurrence at the center counts.
        var hand2 = new[] { "a", "x", "x" };
        Assert.True(HandGeometry.IsCenter(hand2, "x"));
    }

    private static bool IsOdd(int n) => n % 2 == 1;

    [Fact]
    public void IsAdjacentToMatch_FirstPositionChecksOnlyRightNeighbour()
    {
        Assert.True(HandGeometry.IsAdjacentToMatch(new[] { 2, 1 }, 2, IsOdd));
        Assert.False(HandGeometry.IsAdjacentToMatch(new[] { 2, 4 }, 2, IsOdd));
    }

    [Fact]
    public void IsAdjacentToMatch_LastPositionChecksOnlyLeftNeighbour()
    {
        Assert.True(HandGeometry.IsAdjacentToMatch(new[] { 1, 2 }, 2, IsOdd));
        Assert.False(HandGeometry.IsAdjacentToMatch(new[] { 4, 2 }, 2, IsOdd));
    }

    [Fact]
    public void IsAdjacentToMatch_SingleCardHandHasNoNeighbours()
    {
        Assert.False(HandGeometry.IsAdjacentToMatch(new[] { 1 }, 1, _ => true));
    }

    [Fact]
    public void IsAdjacentToMatch_MatchOnEitherSide()
    {
        Assert.True(HandGeometry.IsAdjacentToMatch(new[] { 1, 2, 4 }, 2, IsOdd));
        Assert.True(HandGeometry.IsAdjacentToMatch(new[] { 4, 2, 1 }, 2, IsOdd));
    }

    [Fact]
    public void IsAdjacentToMatch_MatchTwoAwayDoesNotCount()
    {
        Assert.False(HandGeometry.IsAdjacentToMatch(new[] { 1, 4, 2, 6 }, 2, IsOdd));
        Assert.False(HandGeometry.IsAdjacentToMatch(new[] { 2, 4, 1 }, 2, IsOdd));
    }

    [Fact]
    public void IsAdjacentToMatch_AbsentItemIsNeverAdjacent()
    {
        Assert.False(HandGeometry.IsAdjacentToMatch(new[] { 1, 3, 5 }, 2, _ => true));
        Assert.False(HandGeometry.IsAdjacentToMatch(Array.Empty<int>(), 2, _ => true));
    }

    [Fact]
    public void IsAdjacentToMatch_ItemItselfMatchingDoesNotCount()
    {
        Assert.False(HandGeometry.IsAdjacentToMatch(new[] { 2, 1, 2 }, 1, n => n == 1));
    }
}
