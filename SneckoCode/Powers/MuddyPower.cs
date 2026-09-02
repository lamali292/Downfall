using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;

namespace Snecko.SneckoCode.Powers;

public class MuddyPower : SneckoPowerModel
{
    public MuddyPower()
    {
        WithTip(SneckoKeywords.Muddle);
    }
    
    
    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner) || AmountOnTurnStart == 0 || Owner.Player == null) return;
        await SneckoCmd.Muddle(new BlockingPlayerChoiceContext(), Owner.Player.Hand, this);
        Flash();
        await PowerCmd.Decrement(this);
    }
}