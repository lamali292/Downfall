using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Rare;

/// <summary>
///     Heal 10 HP. Ethereal. Exhaust.
///     Upgrade: Heal 13 HP.
/// </summary>
public sealed class Reprieve : HermitCardModel
{
    private const int HealAmount = 10;
    private const int UpgradedHealAmount = 13;

    public Reprieve() : base(2, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
        WithHeal(10);
        WithKeyword(CardKeyword.Ethereal);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(UpgradedHealAmount - HealAmount);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Rare
 *   usings updated
 *   CanonicalVars removed → With* calls in constructor
 *   CanonicalKeywords removed → WithKeyword(...) in constructor
 *   constructor: WithHeal(10, 0), WithKeyword(CardKeyword.Ethereal), WithKeyword(CardKeyword.Exhaust)
 */