using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Powers;

public class SizzlePower : HexaghostPowerModel
{
    public SizzlePower()
    {
        WithTip(CardKeyword.Exhaust);
    }
    
    public override CardLocation ModifyCardPlayResultLocation(CardModel card, bool isAutoPlay,
        ResourceInfo resources, CardLocation cardLocation)
    {
        if (card.Owner.Creature != Owner || cardLocation.pileType != PileType.Discard)
            return cardLocation;
        return cardLocation with { pileType = PileType.Exhaust };
    }

    public override async Task AfterModifyingCardPlayResultLocation(CardModel card,
        CardLocation cardLocation)
    {
        if (card.Owner.Creature != Owner) return;
        Flash();
        await PowerCmd.Decrement(this);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return;
        await PowerCmd.Remove(this);
    }
}