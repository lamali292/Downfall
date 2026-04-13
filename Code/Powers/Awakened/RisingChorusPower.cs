using System.Globalization;
using Downfall.Code.Abstract;
using Downfall.Code.Commands;
using Downfall.Code.Events;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Powers.Awakened;

public class RisingChorusPower : AwakenedPowerModel, IOnChant, IHasSecondAmount
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("UsesLeft", 0)];

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