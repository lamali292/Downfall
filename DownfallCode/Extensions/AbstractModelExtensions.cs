using BaseLib.Abstracts;
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
            EnchantmentModel enchantment => enchantment.Card.GetCreature(),
            AfflictionModel affliction => affliction.Card.GetCreature(),
            CardModifier cardModifier => cardModifier.Owner?.GetCreature() ?? throw new ArgumentException($"Unknown model type: {model.GetType().Name}"),
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
            EnchantmentModel enchantment => enchantment.DynamicVars,
            AfflictionModel affliction => affliction.Card.DynamicVars,
            CardModifier cardModifier => cardModifier.DynamicVars,
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
        if (model is not CardModel card) return model.ResolveTargets(model.GetTargetType(), singleTarget);
        var targetType = card.TargetType;
        if (targetType is TargetType.AnyEnemy or TargetType.AnyAlly or TargetType.AnyPlayer
            || CustomTargetType.IsCustomSingleTargetType(targetType))
            return singleTarget is not null ? [singleTarget] : [];
        return card.GetTargets();

    }

    public static IEnumerable<Creature> MyGetTargets(
        this AbstractModel model, Creature? singleTarget, TargetType targetTypeOverride)
    {
        return model.ResolveTargets(targetTypeOverride, singleTarget);
    }

    private static IEnumerable<Creature> ResolveTargets(
        this AbstractModel model, TargetType type, Creature? singleTarget)
    {
        var creature = model.GetCreature();
        var combatState = creature.CombatState;
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
                combatState.RunState.Rng.CombatTargets.NextItem(combatState.HittableEnemies) is { } enemy
                    ? [enemy] : [],
            _ => throw new InvalidOperationException(
                $"Unsupported TargetType {type} for {model.GetType().Name}")
        };
    }
}