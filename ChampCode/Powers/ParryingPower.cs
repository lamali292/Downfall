using Champ.ChampCode.Cards.Common;
using Champ.ChampCode.Core;
using Champ.ChampCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Champ.ChampCode.Powers;

public class ParryingPower : ChampPowerModel, IModifyCounterStrike
{
    public ParryingPower()
    {
        WithTip<CounterPower>();
        WithCardTip<RiposteStrike>((e, p) =>
        {
            if (p._owner == null) return;
            e.DynamicVars.Damage.BaseValue = p.Owner.GetPowerAmount<CounterPower>();
        });
        WithTip(StaticHoverTip.ReplayStatic);
    }

    public bool ModifyCounterStrike(Player player, RiposteStrike card)
    {
        if (player.Creature != Owner) return false;
        card.BaseReplayCount += Amount;
        return true;
    }

    public async Task AfterModifyingCounterStrike(Player player, RiposteStrike card)
    {
        Flash();
        await PowerCmd.Remove(this);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == Owner.Side) return;
        await PowerCmd.Remove(this);
    }
}