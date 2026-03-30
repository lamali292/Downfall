using Downfall.Code.Abstract;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Awakened;

public class MoonlitVisionPower : AwakenedPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player) return;
        if (cardPlay.Card is not ISpell) return;
        
        var previousSpellsThisTurn = CombatManager.Instance.History.Entries
            .OfType<CardPlayStartedEntry>()
            .Count(e => e.HappenedThisTurn(CombatState) && 
                        e.CardPlay.Card is ISpell && 
                        e.CardPlay != cardPlay);
        if (previousSpellsThisTurn < Amount)
        {
            Flash();
            await PlayerCmd.GainEnergy(1, Owner.Player);
        }
    }
    
    
}