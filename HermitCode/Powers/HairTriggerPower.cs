using Hermit.HermitCode.Cards.Basic;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Powers;

public class HairTriggerPower : HermitPowerModel, IAfterDeadOnTrigger
{
    public HairTriggerPower()
    {
        WithTip(CardKeyword.Exhaust);
        WithTip(HermitKeywords.DeadOn);
        WithCardTip<StrikeHermit>((e, _) => e.AddKeyword(CardKeyword.Exhaust));
    }

    public async Task AfterDeadOnTrigger(PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay)
    {
        if (card.Owner.Creature != Owner) return;
        var canonical = ModelDb.Card<StrikeHermit>();
        var strike = CombatState.CreateCard(canonical, card.Owner);
        strike.AddKeyword(CardKeyword.Exhaust);
        for (var i = 0; i < Amount; i++)
        {
            var clone = strike.CreateClone();
            await CardCmd.AutoPlay(ctx, clone, null);
        }
            
    }
}