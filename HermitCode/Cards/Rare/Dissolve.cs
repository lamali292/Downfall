using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Cards.Rare;

/// <summary>
///     Gain 10 Block. Block not removed for 2 turns. Exhaust.
///     Upgrade: 14 Block.
/// </summary>
public sealed class Dissolve : HermitCardModel
{
    public Dissolve() : base(2, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
        WithBlock(18, 7);
        WithPower<BlurPower>(2);
        WithKeyword(CardKeyword.Exhaust);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        await PowerCmd.Apply<BlurPower>(ctx, Owner.Creature, DynamicVars.Power<BlurPower>().IntValue, Owner.Creature,
            this);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Rare
 *   usings updated
 *   CanonicalKeywords removed → WithKeyword(...) in constructor
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithBlock(18, 7), WithPower<BlurPower>(2, 0), WithKeyword(CardKeyword.Exhaust)
 */