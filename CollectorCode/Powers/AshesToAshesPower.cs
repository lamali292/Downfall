using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

public class AshesToAshesPower : CollectorPowerModel
{
    public AshesToAshesPower()
    {
        WithTip<Ember>();
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)
    {
        if (card is not Ember || card.Owner.Creature != Owner) return;
        Flash();
        foreach (var enemy in CombatState.HittableEnemies.ToList())
            await CompatibilityCreatureCmd.Damage(ctx, enemy, Amount, DamageProps.nonCardHpLoss, Owner, null, null);
    }
}
