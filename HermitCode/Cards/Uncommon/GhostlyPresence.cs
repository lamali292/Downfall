using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Cards.Uncommon;

/// <summary>
///     Gain 7 Block. Dead On: Apply 1 Weak to ALL enemies.
///     Upgrade: 10 Block, 2 Weak.
/// </summary>
public sealed class GhostlyPresence : HermitCardModel, IHasDeadOnEffect
{
    public GhostlyPresence() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
        WithBlock(8, 3);
        WithPower<WeakPower>(1, 1);
    }

   


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }

    public async Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        foreach (var enemy in CombatState!.HittableEnemies)
            await PowerCmd.Apply<WeakPower>(ctx, enemy, DynamicVars["WeakPower"].BaseValue, Owner.Creature, this);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Uncommon
 *   usings updated
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithBlock(8, 3), WithPower<WeakPower>(1, 1)
 */