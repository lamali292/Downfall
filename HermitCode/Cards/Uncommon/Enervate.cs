using BaseLib.Utils;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

/// <summary>
///     Deal 8 damage. Dead On: Gain 1 Energy and draw a card.
///     Upgrade: 12 damage.
/// </summary>
public sealed class Enervate : HermitCardModel, IHasDeadOnEffect
{
    private const int DamageAmount = 7;
    private const int UpgradedDamageAmount = 10;

    public Enervate() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(7, 3);
    }

   

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, play).WithHermitFireHitFx()
            .Execute(ctx);
    }

    public async Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await PlayerCmd.GainEnergy(1, Owner);
        await CardPileCmd.Draw(ctx, 1, Owner);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Uncommon
 *   usings updated
 *   CanonicalVars removed → With* calls in constructor
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithDamage(7, 3)
 *   DamageCmd.Attack chain → CommonActions.CardAttack
 */