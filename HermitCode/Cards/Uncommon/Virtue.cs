using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

/// <summary>
///     Retain. Reduce each stackable debuff on you by 1.
///     Upgrade: Reduce by 2.
/// </summary>
public sealed class Virtue : HermitCardModel
{
    public Virtue() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Retain);
        WithVar("Reduce", 1, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var reduceBy = DynamicVars["Reduce"].IntValue;
        var powers = Owner.Creature.Powers?.ToList();
        if (powers == null) return;

        foreach (var power in powers.Where(power => power.StackType == PowerStackType.Counter))
            switch (power)
            {
                case { Type: PowerType.Debuff, Amount: > 0 } when power.Amount <= reduceBy:
                    await PowerCmd.Remove(power);
                    break;
                case { Type: PowerType.Debuff, Amount: > 0 }:
                    power.SetAmount(power.Amount - reduceBy);
                    break;
                case { Type: PowerType.Buff, Amount: < 0 } when power.Amount >= -reduceBy:
                    await PowerCmd.Remove(power);
                    break;
                case { Type: PowerType.Buff, Amount: < 0 }:
                    power.SetAmount(power.Amount + reduceBy);
                    break;
            }
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Uncommon
 *   usings updated
 *   CanonicalKeywords removed → WithKeyword(...) in constructor
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithKeyword(CardKeyword.Retain)
 */