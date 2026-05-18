using Hermit.HermitCode.Cards.Curse;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

/// <summary>
///     Gain 8 Block. Add an Impending Doom to your hand.
///     Upgrade: 11 Block.
/// </summary>
public sealed class Midnight : HermitCardModel
{
    public Midnight() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
        WithBlock(12, 3);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        // Add an Impending Doom to hand
        var doom = CombatState?.CreateCard<ImpendingDoom>(Owner);
        if (doom == null) return;
        await CardPileCmd.AddGeneratedCardToCombat(doom, PileType.Hand, Owner);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Uncommon
 *   usings updated
 *   CanonicalVars removed → With* calls in constructor
 *   AdditionalHoverTips/ExtraHoverTips removed (covered by WithPower)
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithBlock(12, 3)
 */