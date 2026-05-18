using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Hermit.HermitCode.Powers;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

/// <summary>
///     Deal 19 damage to ALL enemies. Apply 5 Bruise to ALL enemies.
///     Upgrade: 23 damage. 6 Bruise.
/// </summary>
public sealed class NoHoldsBarred : HermitCardModel
{
    public NoHoldsBarred() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(19, 4);
        WithPower<BruisePower>(5, 1);
        WithEnergy(1);
        WithPower<DrainedPower>(1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, play)
            .WithHermitSlashHitFx()
            .Execute(ctx);
        await MyCommonActions.Apply<BruisePower>(ctx, this, play);
        await CommonActions.ApplySelf<DrainedPower>(ctx, this);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Uncommon
 *   usings updated
 *   CanonicalVars removed → With* calls in constructor
 *   AdditionalHoverTips/ExtraHoverTips removed (covered by WithPower)
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithDamage(19, 4), WithPower<BruisePower>(5, 1)
 */