using Downfall.DownfallCode.Interfaces;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Powers;

public class IntoShadowPower : HexaghostPowerModel, IWheelMoved, IHasSecondAmount
{
    protected override object InitInternalData() => new Data();

    public Task AfterWheelAdvance(PlayerChoiceContext ctx, Player player, CardModel? source, GhostflameModel ghostflame,
        int ghostflameIndex, bool silent) => Task.CompletedTask;

    private int FreeCards
    {
        get => GetInternalData<Data>().FreeCards;
        set {
            GetInternalData<Data>().FreeCards = value;
            if (Amount > 1) InvokeDisplayAmountChanged();
        }
    }
    
    private CardModel? CardSource
    {
        get => GetInternalData<Data>().Source;
        set => GetInternalData<Data>().Source = value;
    }

    
    
    
    public Task AfterWheelRetract(PlayerChoiceContext ctx, Player player, CardModel? source, GhostflameModel ghostflame,
        int ghostflameIndex, bool silent)
    {
        if (Owner != player.Creature) return Task.CompletedTask;
        FreeCards+=Amount;
        return Task.CompletedTask;
    }
    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (ShouldSkip(card)) return false;
        modifiedCost = 0M;
        return true;
    }

    public override bool TryModifyStarCost(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (ShouldSkip(card)) return false;
        modifiedCost = 0M;
        return true;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (ShouldSkip(cardPlay.Card) || !cardPlay.IsLastInSeries) return Task.CompletedTask;
        FreeCards--;
        CardSource = cardPlay.Card;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CardSource != cardPlay.Card) return;
        await CardCmd.Exhaust(ctx, cardPlay.Card);
    }
    

    private bool ShouldSkip(CardModel card)
    {
        if (card.Owner.Creature != Owner) return true;
        var pile = card.Pile?.Type;
        if (pile != PileType.Hand && pile != PileType.Play) return true;
        return FreeCards == 0;
    }

    private class Data
    {
        public int FreeCards;
        public CardModel? Source;
    }

    public string GetSecondAmount()
    {
        return Amount > 1 && FreeCards > 0 ? $"{FreeCards}" : string.Empty;
    }
}