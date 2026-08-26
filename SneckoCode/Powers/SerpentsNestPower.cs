using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Powers;

public class SerpentsNestPower : SneckoPowerModel
{
    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || cardPlay.Card.Type != CardType.Power) return;
        var ctx = new BlockingPlayerChoiceContext();
        await CreatureCmd.Damage(ctx, CombatState.HittableEnemies, Amount, DamageProps.nonCardUnpowered, Owner);
    }
}