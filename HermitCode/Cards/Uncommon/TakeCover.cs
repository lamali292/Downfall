using Hermit.HermitCode.Cards.Basic;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Enchantments;

namespace Hermit.HermitCode.Cards.Uncommon;

/// <summary>
///     Add an upgraded X times Defend to your hand. It costs 0 in the combat.
///     Upgrade: Upgrade the generated Defend an additional time.
/// </summary>
public sealed class TakeCover : HermitCardModel
{
    public TakeCover() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithUpgradingCardTip<DefendHermit>();
        WithTip(typeof(Nimble));
    }

    protected override bool HasEnergyCostX => true;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var defend = CombatState!.CreateCard<DefendHermit>(Owner);
        defend.SetToFreeThisCombat();
        if (IsUpgraded)
            CardCmd.Upgrade(defend);
        CardCmd.Enchant<Nimble>(defend, 3 * EnergyCost.CapturedXValue);
        await CardPileCmd.AddGeneratedCardToCombat(defend, PileType.Hand, Owner);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Uncommon
 *   usings updated
 *   CanonicalKeywords removed → WithKeyword(...) in constructor
 *   constructor: WithKeyword(CardKeyword.Exhaust)
 */