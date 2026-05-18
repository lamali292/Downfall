using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

/// <summary>
///     Gain 12 Block. Dead On: Gain 5 Gold. Exhaust.
///     Upgrade: 15 Block, 10 Gold.
///     (Original STS1: Plated Armor 4/5. Adapted to Block since Plated Armor doesn't exist in STS2.)
/// </summary>
public sealed class Scavenge : HermitCardModel, IHasDeadOnEffect
{
    public Scavenge() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<PlatedArmorPower>(6, 2);
        WithKeyword(CardKeyword.Exhaust);
        WithGold(5, 5);
    }

   

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<PlatedArmorPower>(ctx, this);
    }

    public async Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await PlayerCmd.GainGold(DynamicVars.Gold.BaseValue, Owner);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Uncommon
 *   usings updated
 *   CanonicalVars removed → With* calls in constructor
 *   AdditionalHoverTips/ExtraHoverTips removed (covered by WithPower)
 *   CanonicalKeywords removed → WithKeyword(...) in constructor
 *   OnUpgrade: migrated lines stripped, remainder kept
 *   constructor: WithPower<PlatingPower>(6, 2), WithKeyword(CardKeyword.Exhaust)
 */