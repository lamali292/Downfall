using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;
using MegaCrit.Sts2.Core.Commands;

namespace SlimeBoss.SlimeBossCode.Powers;

// "Whenever you play a 2 Energy Attack, gain 2 Energy and take 3 damage."
public class ThePlaybookPower : SlimeBossPowerModel
{
    private const int EnergyGain = 2;

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || !SlimeBossCmd.IsTwoEnergyAttack(cardPlay.Card)) return;
        await PlayerCmd.GainEnergy(EnergyGain, Owner.Player!);
        await CompatibilityCreatureCmd.Damage(ctx, Owner, Amount, DamageProps.nonCardHpLoss, Owner, null, cardPlay);
    }
}
