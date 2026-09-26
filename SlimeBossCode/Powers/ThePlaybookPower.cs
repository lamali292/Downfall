using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

// "(Innate.) Attacks cost 1 [E] less. Whenever you play an Attack, take 3 damage."
public class ThePlaybookPower : SlimeBossPowerModel
{
    public ThePlaybookPower()
    {
        WithEnergy(0);
    }
    
    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner.Creature != Owner || card.Type != CardType.Attack || originalCost <= 0m) return false;

        modifiedCost = Math.Max(0m, originalCost - DynamicVars.Energy.IntValue);
        return true;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || cardPlay.Card.Type != CardType.Attack) return;
        await CompatibilityCreatureCmd.Damage(ctx, Owner, Amount, DamageProps.nonCardHpLoss, Owner, null, cardPlay);
    }

    public void AddEnergy(int energyIntValue)
    {
        DynamicVars.Energy.BaseValue += energyIntValue;
    }
}
