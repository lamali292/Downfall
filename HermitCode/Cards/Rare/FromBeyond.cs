using Downfall.DownfallCode.Commands;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Cards.Rare;

/// <summary>
///     For each card in your exhaust pile, a random enemy loses 5 HP.
///     Upgrade: 7 HP per card.
/// </summary>
public sealed class FromBeyond : HermitCardModel
{
    private const int HpLossAmount = 5;
    private const int UpgradedHpLossAmount = 7;

    public FromBeyond() : base(1, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
        WithHpLoss(HpLossAmount);
        WithCalculatedVar("CalculatedHits", 0, CountCardsInExhaust);
        WithTip(CardKeyword.Exhaust);
    }

    private static decimal CountCardsInExhaust(CardModel card, Creature? _)
    {
        var exhaustPile = PileType.Exhaust.GetPile(card.Owner);
        return exhaustPile?.Cards.Count ?? 0;
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var enemy = Owner.RunState.Rng.CombatTargets.NextItem(CombatState!.HittableEnemies);
        for (var i = 0; i < CountCardsInExhaust(this, null); i++)
        {
            if (enemy == null) break;
            HermitCombatFx.GroundFireOnTarget(enemy);
            await MyCommonActions.LoseHpToTarget(ctx, this, enemy);
        }
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Rare
 *   usings updated
 *   OnUpgrade removed (all logic migrated to constructor)
 */