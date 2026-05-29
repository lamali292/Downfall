using BaseLib.Extensions;
using BaseLib.Patches.Features;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Extensions;

public static class AbstractModelExtensions
{
    public static Creature GetCreature(this AbstractModel model)
    {
        return model switch
        {
            RelicModel relic => relic.Owner.Creature,
            CardModel card => card.Owner.Creature,
            PotionModel potion => potion.Owner.Creature,
            PowerModel power => power.Owner,
            _ => throw new ArgumentException($"Unknown model type: {model.GetType().Name}")
        };
    }

    public static DynamicVarSet GetDynamicVars(this AbstractModel model)
    {
        return model switch
        {
            RelicModel relic => relic.DynamicVars,
            CardModel card => card.DynamicVars,
            PotionModel potion => potion.DynamicVars,
            PowerModel power => power.DynamicVars,
            _ => throw new ArgumentException($"Unknown model type: {model.GetType().Name}")
        };
    }

    public static TargetType GetTargetType(this AbstractModel model)
    {
        return model switch
        {
            CardModel card => card.TargetType,
            PotionModel potion => potion.TargetType,
            _ => throw new ArgumentException($"Unknown model type: {model.GetType().Name}")
        };
    }

    public static IEnumerable<Creature> MyGetTargets(this AbstractModel model, Creature? singleTarget = null)
    {
        // Cards delegate fully to the existing GetTargets() which handles all cases
        if (model is CardModel card)
        {
            var targetType = card.TargetType;
            if (targetType == TargetType.AnyEnemy || targetType == TargetType.AnyAlly ||
                targetType == TargetType.AnyPlayer || CustomTargetType.IsCustomSingleTargetType(targetType))
                return singleTarget is not null ? [singleTarget] : [];

            return card.GetTargets();
        }

        // Non-card models resolve from creature's combat state
        var creature = model.GetCreature();
        var combatState = creature.CombatState;
        var type = model.GetTargetType();

        return type switch
        {
            TargetType.Self => [creature],
            TargetType.AnyEnemy or TargetType.AnyAlly or TargetType.AnyPlayer =>
                singleTarget is not null ? [singleTarget] : [],
            TargetType.AllEnemies when combatState is not null =>
                combatState.HittableEnemies,
            TargetType.AllAllies when combatState is not null =>
                combatState.PlayerCreatures.Where(c => c is { IsAlive: true }),
            TargetType.RandomEnemy when combatState is not null =>
                combatState.HittableEnemies.TakeRandom(1, combatState.RunState.Rng.CombatTargets),
            _ => throw new InvalidOperationException(
                $"Unsupported TargetType {type} for {model.GetType().Name}")
        };
    }

    public static IEnumerable<Creature> MyGetTargets(
        this AbstractModel model, Creature? singleTarget, TargetType targetTypeOverride)
    {
        var creature = model.GetCreature();
        var combatState = creature.CombatState;

        return targetTypeOverride switch
        {
            TargetType.Self => [creature],
            TargetType.AnyEnemy or TargetType.AnyAlly or TargetType.AnyPlayer =>
                singleTarget is not null ? [singleTarget] : [],
            TargetType.AllEnemies when combatState is not null =>
                combatState.HittableEnemies,
            TargetType.AllAllies when combatState is not null =>
                combatState.PlayerCreatures.Where(c => c is { IsAlive: true }),
            TargetType.RandomEnemy when combatState is not null =>
                combatState.HittableEnemies.TakeRandom(1, combatState.RunState.Rng.CombatTargets),
            _ => throw new InvalidOperationException(
                $"Unsupported TargetType {targetTypeOverride} for {model.GetType().Name}")
        };
    }
}