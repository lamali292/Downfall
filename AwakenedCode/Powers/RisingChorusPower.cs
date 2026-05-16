using System.Globalization;
using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Events;
using Awakened.AwakenedCode.Interfaces;
using BaseLib.Abstracts;
using Downfall.DownfallCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Powers;

public class RisingChorusPower : AwakenedPowerModel, IOnChant, IHasSecondAmount
{
    public RisingChorusPower()
    {
        WithVar("UsesLeft", 0);
    }

    public string GetSecondAmount()
    {
        return (Amount - DynamicVars["UsesLeft"].BaseValue).ToString(CultureInfo.InvariantCulture);
    }

    public async Task OnCardChanted(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (card.Owner.Creature != Owner || card is not IChantable) return;
        if (DynamicVars["UsesLeft"].BaseValue < Amount)
        {
            DynamicVars["UsesLeft"].BaseValue++;
            InvokeDisplayAmountChanged();
            Flash();

            await AwakenedCmd.Chant(ctx, card, cardPlay);
        }
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return Task.CompletedTask;
        DynamicVars["UsesLeft"].BaseValue = 0;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}