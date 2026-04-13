using Downfall.Code.Abstract;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Powers.Awakened;

public class ArchmagusPower : AwakenedPowerModel, IHasSecondAmount
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public string GetSecondAmount()
    {
        var am = Amount - SpellsPlayedThisTurn;
        return am <= 0 ? "0" : am.ToString();
    }
    
    private int SpellsPlayedThisTurn => CombatManager.Instance.History.CardPlaysStarted.Count(e =>
        e.Actor == Owner &&
        e.CardPlay is { IsFirstInSeries: true, Card: ISpell } &&
        e.HappenedThisTurn(CombatState));
    
    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        return card.Owner.Creature != Owner || card is not ISpell || SpellsPlayedThisTurn >= Amount
            ? playCount
            : playCount + 1;
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature == Owner)
        {
            InvokeDisplayAmountChanged();
        }
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (card.Owner.Creature == Owner && card is ISpell)
        {
            InvokeDisplayAmountChanged();
        }
        return Task.CompletedTask;
    }
    
    
    public override Task AfterModifyingCardPlayCount(CardModel card)
    {
        return Task.CompletedTask;
    }
}