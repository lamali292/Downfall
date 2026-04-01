using Downfall.Code.Abstract;
using Downfall.Code.Commands;
using Downfall.Code.Displays;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Downfall.Code.Powers.Awakened;

#pragma warning disable STS001
public class AwakenMeterPower : AwakenedPowerModel
#pragma warning restore STS001
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IsVisibleInternal => false;

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player) return;
        if (cardPlay.Card.Type != CardType.Power) return;
        if (AwakenedCmd.IsAwakened(Owner)) return;

        await PowerCmd.Apply<AwakenMeterPower>(Owner, 1, Owner, null);

        if (Amount >= 8)
            await AwakenedCmd.Awaken(Owner.Player!, ctx);
    }
    
    // Todo : why is this here. Cant that be at a different place. here is it always loaded i guess.
    // Temporary just triggers drained on void card drawn
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.CombatState == null) return;
        if( card.Owner != Owner.Player) return;
        if (card is Void)
        {
            foreach (var model in card.CombatState.IterateHookListeners().OfType<IOnDrained>())
                await model.OnDrained(card.Owner, 1);
        }
    }

}