using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Cards.Uncommon;

/// <summary>
///     Gain 5 Block. ALL enemies lose 1 Strength. Exhaust.
///     Upgrade: ALL enemies lose 2 Strength.
/// </summary>
public sealed class FlashPowder : HermitCardModel
{
    public FlashPowder() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithBlock(5);
        WithKeyword(CardKeyword.Exhaust);
        WithVar("StrengthLoss", 1, 1);
        WithTip(typeof(StrengthPower));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        foreach (var enemy in CombatState!.HittableEnemies)
            await PowerCmd.Apply<StrengthPower>(ctx, enemy, -DynamicVars["StrengthLoss"].BaseValue, Owner.Creature,
                this);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Uncommon
 *   usings updated
 *   CanonicalKeywords removed → WithKeyword(...) in constructor
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithBlock(5, 0), WithKeyword(CardKeyword.Exhaust)
 */