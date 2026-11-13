using BaseLib.Extensions;
using BaseLib.Patches.Features;
using BaseLib.Utils;
using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.DownfallCode.Commands;

public static class MyCommonActions
{
    public static Task<T?> ApplySelf<T>(PlayerChoiceContext ctx, AbstractModel model)
        where T : PowerModel
    {
        return PowerCmd.Apply<T>(ctx, model.Creature, model.DynamicVars.Power<T>().BaseValue, model.Creature,
            model as CardModel);
    }

    public static Task Block(AbstractModel model, CardPlay? play = null)
    {
        var dynamicVars = model.DynamicVars;
        var creature = model.Creature;

        if (dynamicVars.TryGetValue("CalculatedBlock", out var calculatedVar) &&
            calculatedVar is CalculatedBlockVar calculatedBlock)
            return CreatureCmd.GainBlock(creature, calculatedBlock.Calculate(play?.Target), calculatedBlock.Props,
                play);

        if (dynamicVars.TryGetValue("Block", out var blockVar) && blockVar is BlockVar block)
            return CreatureCmd.GainBlock(creature, block, play);

        throw new InvalidOperationException(
            $"{model.GetType().Name} does not have a Block or CalculatedBlock var");
    }


    public static async Task<IEnumerable<DamageResult>> SelfDamage(PlayerChoiceContext ctx, AbstractModel model)
    {
        var creature = model.Creature;
        var combatState = creature.CombatState;
        if (combatState == null) return [];
        var damage = model.DynamicVars.SelfDamage;
        var modified = DownfallHook.ModifySelfDamage(combatState, damage.BaseValue, model, out var mod);
        await DownfallHook.AfterModifyingSelfDamage(combatState, mod, model);
        if (modified <= 0) return [];
        return await CreatureCmd.Damage(ctx, model.Creature, modified, damage.Props, model.Creature);
    }

    public static async Task LoseHpToTarget(PlayerChoiceContext ctx, AbstractModel model, Creature target)
    {
        await LoseHpToTarget(ctx, model, [target]);
    }

    public static async Task LoseHpToTarget(
        PlayerChoiceContext ctx, AbstractModel model, IEnumerable<Creature> targets)
    {
        await CompatibilityCreatureCmd.Damage(ctx, targets, model.DynamicVars.HpLoss.BaseValue,
            model is CardModel ? DamageProps.cardHpLoss : DamageProps.nonCardHpLoss, model.Creature, model as CardModel,
            null);
    }

    public static async Task<IReadOnlyList<T>> AutoApply<T>(
        PlayerChoiceContext ctx, AbstractModel model, Creature? target = null)
        where T : PowerModel
    {
        return await Apply<T>(ctx, model, model.MyGetTargets(target).ToList());
    }

    public static async Task<IReadOnlyList<T>> ApplyToAllEnemies<T>(
        PlayerChoiceContext ctx, AbstractModel model)
        where T : PowerModel
    {
        return await Apply<T>(ctx, model, model.Creature.CombatState?.HittableEnemies);
    }


    public static async Task<IReadOnlyList<T>> Apply<T>(
        PlayerChoiceContext ctx, AbstractModel model, IEnumerable<Creature>? targets)
        where T : PowerModel
    {
        if (targets == null) return new List<T>();
        return await PowerCmd.Apply<T>(ctx, targets,
            model.DynamicVars.Power<T>().BaseValue, model.Creature, model as CardModel);
    }

    public static async Task<T?> Apply<T>(
        PlayerChoiceContext ctx, AbstractModel model, Creature? target)
        where T : PowerModel
    {
        if (target == null) return null;
        return await PowerCmd.Apply<T>(ctx, target,
            model.DynamicVars.Power<T>().BaseValue, model.Creature, model as CardModel);
    }

    public static async Task LoseHp(PlayerChoiceContext ctx, AbstractModel model, Creature? target = null)
    {
        await LoseHpToTarget(ctx, model, model.MyGetTargets(target));
    }

    public static AttackCommand Attack(AbstractModel model, Creature? target = null,
        TargetType? targetTypeOverride = null,
        int hitCount = 1, string? vfx = null, string? sfx = null, string? tmpSfx = null)
    {
        var dynamicVars = model.DynamicVars;
        AttackCommand cmd;
        if (dynamicVars.ContainsKey("CalculatedDamage"))
            cmd = DamageCmd.Attack(dynamicVars.CalculatedDamage).WithValueProp(dynamicVars.CalculatedDamage.Props);
        else if (dynamicVars.ContainsKey("Damage"))
            cmd = DamageCmd.Attack(dynamicVars.Damage.BaseValue).WithValueProp(dynamicVars.Damage.Props);
        else
            throw new InvalidOperationException(
                $"{model.GetType().Name} does not have a Damage or CalculatedDamage var");

        cmd.WithHitCount(hitCount);
        cmd.FromModel(model);
        var targets = targetTypeOverride == null
            ? model.MyGetTargets(target).ToList()
            : model.MyGetTargets(target, targetTypeOverride.Value).ToList();

        switch (targets.Count)
        {
            case 0:
                var combatState = model.Creature.CombatState;
                if (combatState == null)
                    throw new InvalidOperationException(
                        $"{model.GetType().Name} requested an AllEnemies attack with no combat state.");
                cmd.TargetingAllOpponents(combatState);
                break;
            case 1:
                cmd.Targeting(targets[0]);
                break;
            case > 1:
                cmd.TargetingFiltered(targets);
                break;
        }

        if (vfx != null || sfx != null || tmpSfx != null)
            cmd.WithHitFx(vfx, sfx, tmpSfx);

        return cmd;
    }

    private static AttackCommand FromModel(this AttackCommand cmd, AbstractModel model)
    {
        if (model is CardModel card)
            return cmd.FromCardCompatibility(card, null);
        if (cmd.Attacker != null)
            throw new InvalidOperationException("Attacker has already been set.");

        cmd.Attacker = model.Creature;
        cmd.ModelSource = model;

        cmd._attackerAnimName = "Attack";
        cmd._sourceType = AttackCommand.SourceType.Card;
        return cmd;
    }

    public static async Task<IEnumerable<CardModel>> Draw(AbstractModel card, PlayerChoiceContext context)
    {
        var player = card.Creature.Player;
        if (player == null) return [];
        return await CardPileCmd.Draw(context, card.DynamicVars.Cards.BaseValue, player);
    }
}