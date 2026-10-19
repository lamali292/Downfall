using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

// "(Innate.) Attacks cost 0 [E]. Whenever you play an Attack, take 3 damage."
public class ThePlaybookPower : SlimeBossPowerModel
{
    public override bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (card.Owner.Creature != Owner || card.Type != CardType.Attack)
        {
            modifiedCost = originalCost;
            return false;
        }
        modifiedCost = 0m;
        return true;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || cardPlay.Card.Type != CardType.Attack) return;
        await CompatibilityCreatureCmd.Damage(ctx, Owner, Amount, DamageProps.nonCardHpLoss, Owner, null, cardPlay);
    }
}
