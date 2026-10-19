using Downfall.DownfallCode.Compatibility;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Powers;

public class IntoShadowPower : HexaghostPowerModel, IWheelMoved, IModifyCardPlayResultLocation
{
    
    public Task AfterWheelAdvance(PlayerChoiceContext ctx, Player player, AbstractModel? source,
        GhostflameModel ghostflame,
        int ghostflameIndex, bool endOfTurnAdvance)
    {
        return Task.CompletedTask;
    }

    public override int DisplayAmount => FreeCards;

    public Task AfterWheelRetract(PlayerChoiceContext ctx, Player player, AbstractModel? source,
        GhostflameModel ghostflame,
        int ghostflameIndex)
    {
        if (Owner != player.Creature) return Task.CompletedTask;
        FreeCards += Amount;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    private int FreeCards { get; set; }

    public override bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner.Creature != Owner || FreeCards <= 0) return false;
        modifiedCost = 0M;
        return true;
    }

    public override bool TryModifyStarCost(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner.Creature != Owner || FreeCards <= 0) return false;
        modifiedCost = 0M;
        return true;
    }

    // The result location is decided before the card's effect runs, so a charge a card creates by Retracting
    // is never mistaken for the one it is using (same split as Corruption / Free Attack).
    public CardLocationCompatiblity ModifyCardPlayResultLocationCompability(CardModel card, bool isAutoPlay,
        ResourceInfo resources, CardLocationCompatiblity location)
    {
        if (card.Owner.Creature != Owner || FreeCards <= 0 || location.PileType == PileType.None) return location;
        return location with { PileType = PileType.Exhaust };
    }

    // Consumed before the card's effect runs, for the same reason.
    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || !cardPlay.IsFirstInSeries || FreeCards <= 0)
            return Task.CompletedTask;
        FreeCards--;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}
