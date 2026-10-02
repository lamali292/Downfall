namespace Hermit.HermitCode.Core;

/// <summary>
///     Pure hand-position maths behind Dead On and curse adjacency. Deliberately free of game types
///     (generic over the element type, no <c>CardModel</c>) so it can be compiled into a plain .NET
///     test project. Every method takes the ordered hand and the element asked about; an element that
///     is not in the hand is never center and never adjacent to anything.
/// </summary>
public static class HandGeometry
{
    /// <summary>
    ///     Center of the hand: the middle index for an odd hand size, either of the two middle indices
    ///     for an even one. A single-card hand is its own center; an empty hand has none.
    /// </summary>
    public static bool IsCenter(int index, int handSize)
    {
        if (index < 0 || index >= handSize) return false;
        if (handSize % 2 == 0)
            return index == handSize / 2 - 1 || index == handSize / 2;
        return index == handSize / 2;
    }

    public static bool IsCenter<T>(IReadOnlyList<T> hand, T item)
    {
        return IsCenter(IndexOf(hand, item), hand.Count);
    }

    /// <summary>True when the left or right neighbour of <paramref name="item" /> satisfies <paramref name="isMatch" />.</summary>
    public static bool IsAdjacentToMatch<T>(IReadOnlyList<T> hand, T item, Func<T, bool> isMatch)
    {
        var index = IndexOf(hand, item);
        if (index == -1) return false;
        var leftMatches = index > 0 && isMatch(hand[index - 1]);
        var rightMatches = index < hand.Count - 1 && isMatch(hand[index + 1]);
        return leftMatches || rightMatches;
    }

    private static int IndexOf<T>(IReadOnlyList<T> hand, T item)
    {
        var comparer = EqualityComparer<T>.Default;
        for (var i = 0; i < hand.Count; i++)
            if (comparer.Equals(hand[i], item))
                return i;
        return -1;
    }
}
