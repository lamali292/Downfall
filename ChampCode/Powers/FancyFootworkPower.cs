using Champ.ChampCode.Core;
using Champ.ChampCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Champ.ChampCode.Powers;

public class FancyFootworkPower : ChampPowerModel, IOnFinisher
{
    public async Task OnFinisher(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var enemies = CombatState.HittableEnemies;
        await CreatureCmd.Damage(
            ctx,
            enemies,
            Amount,
            ValueProp.Unpowered | ValueProp.Unblockable,
            cardPlay.Card.Owner.Creature);
        await PowerCmd.Remove(this);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Side) return;
        await PowerCmd.Remove(this);
    }
}