using BaseLib.Utils;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Cards.Rare;

/// <summary>
///     Ethereal. Deal damage equal to your missing HP. Exhaust.
///     Upgrade: Cost -1.
/// </summary>
public sealed class BlackWind : HermitCardModel
{
    public BlackWind() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithKeyword(CardKeyword.Ethereal);
        WithKeyword(CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
        WithCalculatedDamage(0, 1, GetLoseHp);
    }

    private static decimal GetLoseHp(CardModel card, Creature? _)
    {
        return card.Owner.Creature.MaxHp - card.Owner.Creature.CurrentHp;
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, play)
            .WithHermitFireHitFx()
            .Execute(ctx);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.FinalizeUpgrade();
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Rare
 *   usings updated
 *   CanonicalKeywords removed → WithKeyword(...) in constructor
 *   OnUpgrade: migrated lines stripped, remainder kept
 *   constructor: WithDamage(0m, 0), WithKeyword(CardKeyword.Ethereal), WithKeyword(CardKeyword.Exhaust), WithCostUpgradeBy(-1)
 *   DamageCmd.Attack chain → CommonActions.CardAttack
 */