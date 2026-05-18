using BaseLib.Utils;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Rare;

/// <summary>
///     Deal 20 damage to ALL enemies. Exhaust.
///     Upgrade: 28 damage.
/// </summary>
public sealed class Purgatory : HermitCardModel
{
    public Purgatory() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithDamage(24, 6);
        WithKeyword(CardKeyword.Ethereal);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, play).WithHermitFireHitFx().Execute(ctx);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Rare
 *   usings updated
 *   CanonicalVars removed → With* calls in constructor
 *   CanonicalKeywords removed → WithKeyword(...) in constructor
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithDamage(24, 6), WithKeyword(CardKeyword.Ethereal)
 */