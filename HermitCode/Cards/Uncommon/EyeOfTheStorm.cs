using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

/// <summary>
///     Concentrate. Gain energy to maximum. Exhaust.
///     Upgrade: Cost reduced from 1 to 0.
/// </summary>
public sealed class EyeOfTheStorm : HermitCardModel
{
    public EyeOfTheStorm() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<ConcentrationPower>(ctx, Owner.Creature, 1, Owner.Creature, this);

        var gain = Owner.PlayerCombatState!.MaxEnergy - Owner.PlayerCombatState.Energy;
        if (gain > 0)
            await PlayerCmd.GainEnergy(gain, Owner);
    }

    protected override void OnUpgrade()
    {
// 1 -> 0
        EnergyCost.FinalizeUpgrade();
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Uncommon
 *   usings updated
 *   CanonicalKeywords removed → WithKeyword(...) in constructor
 *   OnUpgrade: migrated lines stripped, remainder kept
 *   constructor: WithKeyword(CardKeyword.Exhaust), WithCostUpgradeBy(-1)
 */