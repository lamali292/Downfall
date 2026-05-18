using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Relics;

/// <summary>
///     While your HP is at or below 50%, gain Energy and draw 1 card at the start of your turn.
/// </summary>
public sealed class DentedPlate : HermitRelicModel
{
    public DentedPlate() : base(RelicRarity.Uncommon)
    {
        WithEnergy(1);
    }

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != Owner || player.Creature.CurrentHp > player.Creature.MaxHp / 2)
            return count;
        return count + 1;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
    {
        if (player != Owner)
            return;

        if (player.Creature.CurrentHp <= player.Creature.MaxHp / 2)
        {
            Status = RelicStatus.Active;
            Flash();
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, player);
        }
        else
        {
            Status = RelicStatus.Normal;
        }
    }
}