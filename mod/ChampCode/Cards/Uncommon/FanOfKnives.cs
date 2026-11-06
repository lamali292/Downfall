using Downfall.DownfallCode.Compatibility;
using BaseLib.Utils;
using Champ.ChampCode.Core;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class FanOfKnives : ChampCardModel
{
    public FanOfKnives() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(4, 2);
        WithBerserkerCombo();
    }

    protected override Artist Artist => Artist.Get<Opal>();
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        var context = await AttackContextCompatibility.CreateContextAsync(CombatState, ctx, cardPlay);
        var targets = CombatState.HittableEnemies.ToList();
        context.AddHit(await CompatibilityCreatureCmd.Damage(ctx, targets, DynamicVars.Damage.BaseValue, DynamicVars.Damage.Props,
            Owner.Creature, this, cardPlay));

        await ChampCmd.BerserkerCombo(cardPlay, async () =>
        {
            var remaining = CombatState.HittableEnemies.ToList();
            if (remaining.Count == 0) return;
            context.AddHit(await CompatibilityCreatureCmd.Damage(ctx, remaining, DynamicVars.Damage.BaseValue,
                DynamicVars.Damage.Props, Owner.Creature, this, cardPlay));
        });

        await context.DisposeAsync();
    }
}