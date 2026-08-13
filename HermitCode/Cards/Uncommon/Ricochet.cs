using Downfall.DownfallCode.Artists;
using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.History;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class Ricochet : HermitCardModel
{
    public Ricochet() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(8, 2);
        WithCalculatedVar("CalculatedHits", 0, 1, CountDeadOnEffects);
        WithTip(HermitKeywords.DeadOn);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    private static decimal CountDeadOnEffects(CardModel card, Creature? _)
    {
        return CombatManager.Instance.History.Entries.OfType<DeadOnEntry>().Count(e =>
            e.HappenedThisTurn(card.CombatState) && e.Actor == card.Owner.Creature);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        if (play.Target == null) return;
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        var extraHitCount = (int)((CalculatedVar)DynamicVars["CalculatedHits"]).Calculate(play.Target);
        var context = await AttackCommand.CreateContextAsync(CombatState!, ctx, play);
        try
        {
            HermitSfx.PlayGun2();
            var mainHits = (await CreatureCmd.Damage(
                ctx, play.Target, DynamicVars.Damage.BaseValue,
                DamageProps.card,
                this, play)).ToList();
            context.AddHit(mainHits);

            if (extraHitCount > 0)
            {
                HermitSfx.PlayGun3();
                for (var i = 0; i < extraHitCount; i++)
                {
                    var target = RunState!.Rng.CombatTargets.NextItem(CombatState!.HittableEnemies);
                    if (target is not { IsHittable: true }) continue;
                    context.AddHit(await CreatureCmd.Damage(
                        ctx, target, DynamicVars.Damage.BaseValue,
                        DamageProps.card,
                        Owner.Creature, this, play));
                }
            }
        }
        finally
        {
            await context.DisposeAsync();
        }
    }
}