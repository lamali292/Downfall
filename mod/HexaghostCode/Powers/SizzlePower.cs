using Downfall.DownfallCode.Compatibility;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Powers;

public class SizzlePower : HexaghostPowerModel, IModifyCardPlayResultLocation
{
    public SizzlePower()
    {
        WithTip(CardKeyword.Exhaust);
    }
    
    public CardLocationCompatiblity ModifyCardPlayResultLocationCompability(CardModel card, bool isAutoPlay,
        ResourceInfo resources, CardLocationCompatiblity cardLocation)
    {
        if (card.Owner.Creature != Owner || cardLocation.PileType != PileType.Discard)
            return cardLocation;
        return cardLocation with { PileType = PileType.Exhaust };
    }

    public async Task AfterModifyingCardPlayResultLocationCompability(CardModel card,
        CardLocationCompatiblity cardLocation)
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