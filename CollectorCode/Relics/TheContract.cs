using BaseLib.Utils;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class TheContract() : CollectorRelicModel(RelicRarity.Uncommon)
{
    public bool _activatedThisCombat;
    
    public bool ActivatedThisCombat
    {
        get => _activatedThisCombat;
        set
        {
            AssertMutable();
            _activatedThisCombat = value;
        }
    }
    
    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
        {
            return Task.CompletedTask;
        }
        ActivatedThisCombat = false;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!CombatManager.Instance.IsInProgress || cardPlay.Card.Owner != Owner ||
            cardPlay.Card.Type != CardType.Power || ActivatedThisCombat)
        {
            return;
        }

        if (cardPlay.Card.EnergyCost.GetAmountToSpend() >= 0 && cardPlay.Card.EnergyCost.Canonical != -1)//If for some reason you play an unplayable colourless card manually it won't activate.
        {
            await PlayerCmd.GainEnergy(cardPlay.Card.EnergyCost.GetAmountToSpend(), Owner);
            Flash();
            ActivatedThisCombat = true;
        }
    }
}