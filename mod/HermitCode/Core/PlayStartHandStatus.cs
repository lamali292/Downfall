namespace Hermit.HermitCode.Core;

/// <summary>
///     Where a card sat in the hand at the moment its play started: the one value Dead On and curse
///     adjacency are derived from. Captured once per play (before the card leaves the hand) and read
///     by the after-play handler, glow and cards alike, so they cannot disagree.
/// </summary>
public readonly record struct PlayStartHandStatus(bool IsCenter, bool IsAdjacentToCurse)
{
    public static readonly PlayStartHandStatus None = new(false, false);
}
