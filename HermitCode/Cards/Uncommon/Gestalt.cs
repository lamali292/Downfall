using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Cards.Uncommon;

/// <summary>
///     Apply 2 Vulnerable to yourself. Gain 2 Rugged. Exhaust.
///     Upgrade: 1 Vulnerable, 1 Rugged.
/// </summary>
public sealed class Gestalt : HermitCardModel
{
    public Gestalt() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<RuggedPower>(2);
        WithPower<VulnerablePower>(2, -1);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<RuggedPower>(ctx, Owner.Creature, DynamicVars["RuggedPower"].BaseValue, Owner.Creature,
            this);
        await PowerCmd.Apply<VulnerablePower>(ctx, Owner.Creature, DynamicVars["VulnerablePower"].BaseValue,
            Owner.Creature, this);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Uncommon
 *   usings updated
 *   CanonicalKeywords removed → WithKeyword(...) in constructor
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithPower<RuggedPower>(2, 0), WithPower<VulnerablePower>(2, -1), WithKeyword(CardKeyword.Exhaust)
 */