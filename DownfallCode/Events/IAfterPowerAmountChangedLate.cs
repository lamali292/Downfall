using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Events;

/// <summary>
///     Like <c>AbstractModel.AfterPowerAmountChanged</c>, but dispatched only after every regular listener
///     has finished reacting to the change. Use it for effects that must not influence the other
///     reactions to the same change (e.g. a power that adds stacks of another power in response).
/// </summary>
public interface IAfterPowerAmountChangedLate
{
    Task AfterPowerAmountChangedLate(PlayerChoiceContext ctx, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource);
}
