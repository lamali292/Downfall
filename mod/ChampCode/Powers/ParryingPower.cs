using Champ.ChampCode.Cards.Common;
using Champ.ChampCode.Core;
using Champ.ChampCode.Events;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Powers;

public class ParryingPower : ChampPowerModel, IModifyCounterStrike
{
    public ParryingPower()
    {
        WithTip<CounterPower>();
        WithTip(new PowerTooltipSource(GetPowerTooltip));
        WithTip(StaticHoverTip.ReplayStatic);
    }
    
    private static CardHoverTip GetPowerTooltip(PowerModel arg)
    {
        var card = ModelDb.Card<RiposteStrike>().ToMutable();
        card.DynamicVars.Damage.BaseValue = arg.IsMutable ? arg.Owner.GetPowerAmount<CounterPower>() : 0;
        return new CardHoverTip(card);
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