using BaseLib.Utils;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Rare;

public class Roughhouse : HermitCardModel, IHasDeadOnEffect
{
    public Roughhouse() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(24, 6);
        WithBlock(20, 4);
    }

   

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, play).WithHermitBluntHeavyHitFx()
            .Execute(ctx);
    }

    public async Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
    }
}

/* transform_cards.py changes:
 *   primary constructor → explicit constructor + base(...)
 *   namespace → Hermit.HermitCode.Cards.Rare
 *   usings updated
 *   CanonicalVars removed → With* calls in constructor
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithDamage(24, 6), WithBlock(20, 4)
 *   DamageCmd.Attack chain → CommonActions.CardAttack
 */