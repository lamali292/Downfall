using Downfall.Code.Abstract;
using Downfall.Code.Interfaces;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Powers.Awakened;

public class RisingChorusPower : AwakenedPowerModel, IOnChant 
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("UsesLeft", 0)];

    public async Task OnCardChanted(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (card.Owner.Creature != Owner || card is not IChantable chantable) return;
        if (DynamicVars["UsesLeft"].BaseValue < Amount)
        {
            DynamicVars["UsesLeft"].BaseValue++;
            Flash();
            await chantable.OnChant(ctx, cardPlay);
        }
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return Task.CompletedTask;
        DynamicVars["UsesLeft"].BaseValue = 0;
        return Task.CompletedTask;
    }
}