using Awakened.AwakenedCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Awakened.AwakenedCode.Powers;

public class EclipseEmbracePower : AwakenedPowerModel
{
    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card,
        bool causedByEthereal)
    {
        if (card.Owner != Owner.Player || card is not Void) return;
        await PlayerCmd.GainEnergy(Amount, Owner.Player);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(ctx, Owner, Amount, Owner, null);
    }
}