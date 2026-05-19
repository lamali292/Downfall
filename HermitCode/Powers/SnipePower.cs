using Hermit.HermitCode.Core;
using Hermit.HermitCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Powers;

public sealed class SnipePower : HermitPowerModel, IModifyDeadOnCount
{
  
    public int ModifyDeadOnCount(int amount, CardModel card)
     => card.Owner.Creature == Owner ? amount + 1 : amount;

    public async Task AfterModifyingDeadOnCount(PlayerChoiceContext ctx, CardModel card)
    {
        await PowerCmd.ModifyAmount(ctx, this, -1, Owner, card);
    }
    
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side) return;
        await PowerCmd.Remove(this);
    }



}