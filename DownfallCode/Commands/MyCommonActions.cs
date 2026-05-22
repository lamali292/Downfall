using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.DownfallCode.Commands;

public static class MyCommonActions
{
    public static async Task CardCalculatedBlock(CardModel card, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(card, card.DynamicVars.CalculatedBlock, cardPlay);
    }

    public static Task<IEnumerable<DamageResult>> SelfDamage(PlayerChoiceContext ctx, CardModel card)
    {
        return CreatureCmd.Damage(ctx, card.Owner.Creature, card.DynamicVars.SelfDamage(), card);
    }

    public static async Task LoseHpToTarget(PlayerChoiceContext ctx, CardModel card, Creature target)
    {
        await CreatureCmd.Damage(ctx, target, card.DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered, card.Owner.Creature, card);
    }

    public static async Task LoseHpToTarget(PlayerChoiceContext ctx, CardModel card, IEnumerable<Creature> targets)
    {
        await CreatureCmd.Damage(ctx, targets, card.DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered, card.Owner.Creature, card);
    }

    public static async Task LoseHp(PlayerChoiceContext ctx, CardModel card, CardPlay? cardPlay)
    {
        switch (card)
        {
            case { TargetType: TargetType.Self }:
                await LoseHpToTarget(ctx, card, card.Owner.Creature);
                break;
            case { TargetType: TargetType.AnyEnemy or TargetType.AnyAlly or TargetType.AnyPlayer }:
                if (cardPlay?.Target is not null) await LoseHpToTarget(ctx, card, cardPlay.Target);
                break;
            case { TargetType: TargetType.AllEnemies, CombatState: not null }:
                var enemies = card.CombatState.HittableEnemies.ToList();
                await LoseHpToTarget(ctx, card, enemies);
                break;
            case { TargetType: TargetType.RandomEnemy, CombatState: not null }:
                var enemy = card.CombatState?.HittableEnemies.TakeRandom(1, card.CombatState.RunState.Rng.CombatTargets)
                    .FirstOrDefault();
                if (enemy != null) await LoseHpToTarget(ctx, card, enemy);
                break;
            default:
                throw new InvalidOperationException($"Unsupported TargetType {card.TargetType} for LoseHp action.");
        }
    }

    public static Task ApplySelf<T>(PlayerChoiceContext ctx, RelicModel model)
        where T : PowerModel
    {
        return PowerCmd.Apply<T>(ctx, model.Owner.Creature, model.DynamicVars.Power<T>().BaseValue,
            model.Owner.Creature,
            null);
    }
}