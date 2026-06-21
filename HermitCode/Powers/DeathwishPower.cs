using Hermit.HermitCode.Core;
using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Events;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Powers;

public class DeathwishPower : HermitPowerModel, IShouldTriggerDeadOn
{
    public DeathwishPower() : base(PowerType.Buff, PowerStackType.Single)
    {
        WithTip(HermitKeywords.DeadOn);
    }

    public bool ShouldTriggerDeadOn(CardModel card)
    {
        if (card.Owner.Creature != Owner) return false;
        var hand = PileType.Hand.GetPile(card.Owner).Cards.ToList();
        var idx = hand.IndexOf(card);
        if (idx == -1) return false;

        var leftIsCurse  = idx > 0 && IsCurse(hand[idx - 1]);
        var rightIsCurse = idx < hand.Count - 1 && IsCurse(hand[idx + 1]);
        return leftIsCurse || rightIsCurse;
    }
    private static bool IsCurse(CardModel c) => c.Type == CardType.Curse;
}