using Hexaghost.HexaghostCode.Core;

namespace Hexaghost.HexaghostCode.Interfaces;

/// <summary>
/// An Afterlife effect that, when triggered by Ethereal at end of turn, resolves before the side's turn-end hooks
/// (<see cref="HexaghostModel.BeforeSideTurnEndVeryEarly"/>) instead of on exhaust. Use it for effects that
/// end-of-turn powers such as Plating must see. Any other exhaust still resolves on exhaust as usual.
/// </summary>
public interface IHasEarlyAfterlifeEffect : IHasAfterlifeEffect;
