using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Awakened.AwakenedCode.Powers;

public class AphoticFountPower : AwakenedPowerModel
{
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player) return;
        if (cardPlay.Card is not Cryostasis) return;
        await PowerCmd.Apply<PlatingPower>(ctx, Owner, Amount, Owner, null);
    }
}