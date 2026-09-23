using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Downfall.DownfallCode.Extensions;

/// <summary>
/// Implemented by a submod's own top-level <c>AbstractModel</c> subtype (not a card/power/relic/
/// potion/enchantment/affliction/modifier - those are already handled directly) so that
/// <see cref="AbstractModelExtensions"/>'s <c>.DynamicVars</c>/<c>.Creature</c>/<c>.Player</c>
/// extensions work on it without DownfallCode hardcoding that submod's type, e.g. Hexaghost's
/// <c>GhostflameModel</c>.
/// </summary>
public interface ICustomAbstractModel
{
    DynamicVarSet DynamicVars { get; }
    Creature Creature { get; }
    Player Player { get; }
}
