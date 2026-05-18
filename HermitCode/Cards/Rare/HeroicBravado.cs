using BaseLib.Extensions;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Rare;

/// <summary>
///     Ethereal. Gain 1 Rugged. Reduce this card's cost by 2 this combat.
///     Upgrade: Reduce cost by 1 instead.
/// </summary>
public sealed class HeroicBravado : HermitCardModel
{
    private const int CostIncrease = 2;
    private const int UpgradedCostIncrease = 1;

    public HeroicBravado() : base(1, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
        WithKeyword(CardKeyword.Ethereal);
        WithPower<RuggedPower>(1);
        WithVar("CostIncrease", 2, -1);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var increase = DynamicVars["CostIncrease"].IntValue;
        var newCost = Math.Max(0, EnergyCost.GetWithModifiers(default) + increase);
        EnergyCost.SetCustomBaseCost(newCost);

        await PowerCmd.Apply<RuggedPower>(ctx, Owner.Creature, DynamicVars.Power<RuggedPower>().IntValue,
            Owner.Creature, this);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Rare
 *   usings updated
 *   CanonicalKeywords removed → WithKeyword(...) in constructor
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithKeyword(CardKeyword.Ethereal)
 */