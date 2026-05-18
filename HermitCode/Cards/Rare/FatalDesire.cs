using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Cards.Rare;

/// <summary>
///     At the start of your turn, draw 2 cards and add an Injury to your hand.
///     Upgrade: Cost 0.
/// </summary>
public sealed class FatalDesire : HermitCardModel
{
    private const int Cards = 2;

    public FatalDesire() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithCards(2);
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<MachineLearningPower>(ctx, Owner.Creature, DynamicVars.Cards.BaseValue, Owner.Creature,
            this);
        await PowerCmd.Apply<FatalDesirePower>(ctx, Owner.Creature, 1, Owner.Creature, this);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Rare
 *   usings updated
 *   CanonicalVars removed → With* calls in constructor
 *   AdditionalHoverTips/ExtraHoverTips removed (covered by WithPower)
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithCards(2, 0), WithKeyword(CardKeyword.Innate, UpgradeType.Add)
 */