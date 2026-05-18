using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Hermit.HermitCode.Powers;

/// <summary>
///     At the start of your turn, lose 1 energy.
/// </summary>
public sealed class DrainedPower() : HermitPowerModel(PowerType.Debuff)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];

    protected override async Task AfterEnergyReset(PlayerChoiceContext ctx, Player player)
    {
        if (player == Owner.Player)
        {
            await PlayerCmd.LoseEnergy(Amount, player);
            await PowerCmd.Remove(this);
        }
    }
}