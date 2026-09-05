using BaseLib.Abstracts;
using Downfall.DownfallCode.Compatibility;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Powers;

public class IntoShadowPower : HexaghostPowerModel, IWheelMoved
{
    
    public Task AfterWheelAdvance(PlayerChoiceContext ctx, Player player, AbstractModel? source,
        GhostflameModel ghostflame,
        int ghostflameIndex, bool silent)
    {
        return Task.CompletedTask;
    }

    public override int DisplayAmount => FreeCards;

    public Task AfterWheelRetract(PlayerChoiceContext ctx, Player player, AbstractModel? source,
        GhostflameModel ghostflame,
        int ghostflameIndex, bool silent)
    {
        if (Owner != player.Creature) return Task.CompletedTask;
        Source = source;
        FreeCards += Amount;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
    
    private AbstractModel? Source { get; set; }
    private int FreeCards { get; set; }

    public override bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner.Creature != Owner || FreeCards == 0 || Source == card) return false;
        modifiedCost = 0M;
        return true;
    }

    public override bool TryModifyStarCost(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner.Creature != Owner || FreeCards == 0 || Source == card) return false;
        modifiedCost = 0M;
        return true;
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {   
        if (FreeCards == 0 || cardPlay.Card == Source) return;
     
        if (cardPlay.ResultPile is not PileType.None) await CardCmdCompatibility.Exhaust(ctx, cardPlay.Card);
        FreeCards--;
        InvokeDisplayAmountChanged();
        Source = null;
    }

   
}