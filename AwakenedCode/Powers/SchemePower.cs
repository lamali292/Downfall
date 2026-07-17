using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Powers;

public class SchemePower : AwakenedPowerModel
{
    
    // TODO: refactor. actually replay maybe
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player || cardPlay.IsAutoPlay || cardPlay.Card is Scheme) return;
        var dupe = cardPlay.Card.CreateDupeCompat();
        await CardCmd.AutoPlay(ctx, dupe, cardPlay.Target);
        await PowerCmd.Decrement(this);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return;
        await PowerCmd.Remove(this);
    }
}