using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Cards.Common;

/// <summary>
///     Apply 1 Weak and 1 Vulnerable to an enemy.
///     Upgrade: Retain.
/// </summary>
public sealed class Glare : HermitCardModel
{
    public Glare() : base(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithPower<WeakPower>(1);
        WithPower<VulnerablePower>(1);
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await MyCommonActions.Apply<WeakPower>(ctx, this, play);
        await MyCommonActions.Apply<VulnerablePower>(ctx, this, play);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Common
 *   usings updated
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithPower<WeakPower>(1m, 0), WithPower<VulnerablePower>(1m, 0), WithKeyword(CardKeyword.Retain, UpgradeType.Add)
 */