using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

/// <summary>
///     Exhaust all Unplayable cards in hand. Gain 8 Block. Draw 3 card.
///     Upgrade: 10 Block, Draw 4.
/// </summary>
public sealed class Spite : HermitCardModel
{
    public Spite() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
        WithBlock(8, 2);
        WithCards(3, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        if (Owner.PlayerCombatState == null) return;
        var unplayable = Owner.PlayerCombatState.Hand.Cards
            .Where(c => c.Keywords.Contains(CardKeyword.Unplayable))
            .ToList();
        await CardPileCmd.Add(unplayable, PileType.Exhaust);
        await CommonActions.CardBlock(this, play);
        await CommonActions.Draw(this, ctx);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Uncommon
 *   usings updated
 *   CanonicalVars removed → With* calls in constructor
 *   AdditionalHoverTips/ExtraHoverTips removed (covered by WithPower)
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithBlock(8, 2), WithCards(3, 1)
 */