using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Powers;

public class StunnedPower() : DownfallPowerModel(PowerType.Debuff, PowerStackType.Single)
{
    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        return player.Creature == Owner ? 0 : count;
    }

    protected override async Task AfterEnergyReset(PlayerChoiceContext ctx, Player player)
    {
        if (player.Creature != Owner) return;
        Flash();
        await PlayerCmd.SetEnergy(0, player);
    }

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        return autoPlayType != AutoPlayType.None || card.Owner.Creature != Owner;
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext ctx, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}

// TODO: prevent player from playing potions