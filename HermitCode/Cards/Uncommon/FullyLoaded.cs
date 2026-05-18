using Hermit.HermitCode.CustomEnums;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

/// <summary>
///     Add ALL Strikes and Defends from your draw pile to your hand. Exhaust.
///     Upgrade: Cost reduced from 2 to 1.
/// </summary>
public sealed class FullyLoaded : HermitCardModel
{
    public FullyLoaded() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
        WithTip(HermitKeywords.Strike);
        WithTip(HermitKeywords.Defend);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        HermitSfx.PlaySpin();
        HermitSfx.PlayReload();
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var strikesAndDefends = Owner.PlayerCombatState?.DrawPile.Cards
            .Where(c => c.Tags.Contains(CardTag.Strike) || c.Tags.Contains(CardTag.Defend))
            .ToList() ?? [];
        await CardPileCmd.Add(strikesAndDefends, PileType.Hand);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Uncommon
 *   usings updated
 *   CanonicalKeywords removed → WithKeyword(...) in constructor
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithKeyword(CardKeyword.Exhaust), WithKeyword(CardKeyword.Retain, UpgradeType.Add)
 */