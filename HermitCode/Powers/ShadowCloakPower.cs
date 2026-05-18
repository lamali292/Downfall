using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Powers;

public sealed class ShadowCloakPower : HermitPowerModel
{
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner.Creature != Owner || card.Type != CardType.Curse)
            return;
        await CreatureCmd.TriggerAnim(Owner, "Cast", Owner.Player!.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card,
        bool causedByEthereal)
    {
        if (card.Owner.Creature != Owner || card.Type != CardType.Curse)
            return;
        await CreatureCmd.TriggerAnim(Owner, "Cast", Owner.Player!.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
    }
}