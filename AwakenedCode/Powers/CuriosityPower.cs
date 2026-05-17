using Awakened.AwakenedCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Awakened.AwakenedCode.Powers;

public class CuriosityPower : AwakenedPowerModel
{
    private bool _isFirstTriggered = true;

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (Owner.Player != cardPlay.Card.Owner || cardPlay.Card.Type != CardType.Power)
            return;
        
        if (_isFirstTriggered)
        {
            _isFirstTriggered = false;
            return;
        }
        await PowerCmd.Apply<StrengthPower>(ctx, Owner, Amount, Owner, null);
    }
}