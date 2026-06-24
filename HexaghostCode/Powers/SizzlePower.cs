using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Powers;

public class SizzlePower : HexaghostPowerModel
{
    public SizzlePower()
    {
        WithTip(CardKeyword.Exhaust);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (card.Owner.Creature != Owner) return;
        await CardCmd.Exhaust(ctx, card);
        await PowerCmd.Decrement(this);
    }


    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        
        if (!participants.Contains(Owner))
            return;
        await PowerCmd.Remove(this);
    }
}