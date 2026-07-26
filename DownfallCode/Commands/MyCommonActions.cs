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
        var creature = model.GetCreature();
        var dynamicVars = model.GetDynamicVars();
        return PowerCmd.Apply<T>(ctx, creature, dynamicVars.Power<T>().BaseValue, creature, model as CardModel);
    }

    public static Task Block(AbstractModel model, CardPlay? play = null)
    {
        var dynamicVars = model.GetDynamicVars();
        var creature = model.GetCreature();

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
        var creature = model.GetCreature();
        var combatState = creature.CombatState;
        if (combatState == null) return [];
        var damage = model.GetDynamicVars().SelfDamage();
        var modified = DownfallHook.ModifySelfDamage(combatState, damage.BaseValue, model, out var mod);
        await DownfallHook.AfterModifyingSelfDamage(combatState, mod, model);
        if (modified <= 0) return [];
        return await CreatureCmd.Damage(ctx, model.GetCreature(), modified, damage.Props, model.GetCreature());
    }

    public static async Task LoseHpToTarget(PlayerChoiceContext ctx, AbstractModel model, Creature target)
    {
        await DownfallCreatureCmd.Damage(ctx, target, model.GetDynamicVars().HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered, model.GetCreature(), model as CardModel, null);
    }

    public static async Task LoseHpToTarget(
        PlayerChoiceContext ctx, AbstractModel model, IEnumerable<Creature> targets)
    {
        await DownfallCreatureCmd.Damage(ctx, targets, model.GetDynamicVars().HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered, model.GetCreature(), model as CardModel, null);
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
        return await Apply<T>(ctx, model, model.GetCreature().CombatState?.HittableEnemies);
    }
    
    
    public static async Task<IReadOnlyList<T>> Apply<T>(
        PlayerChoiceContext ctx, AbstractModel model, IEnumerable<Creature>? targets)
        where T : PowerModel
    {
        return await PowerCmd.Apply<T>(ctx, targets,
            model.GetDynamicVars().Power<T>().BaseValue, model.GetCreature(), model as CardModel);
    }
    
    public static async Task<T?> Apply<T>(
        PlayerChoiceContext ctx, AbstractModel model, Creature? target)
        where T : PowerModel
    {
        if (target == null) return null;
        return await PowerCmd.Apply<T>(ctx, target,
            model.GetDynamicVars().Power<T>().BaseValue, model.GetCreature(), model as CardModel);
    }

    public static async Task LoseHp(PlayerChoiceContext ctx, AbstractModel model, Creature? target = null)
    {
        await LoseHpToTarget(ctx, model, model.MyGetTargets(target));
    }

    public static AttackCommand Attack(AbstractModel model, Creature? target = null,
        TargetType? targetTypeOverride = null,
        int hitCount = 1, string? vfx = null, string? sfx = null, string? tmpSfx = null)
    {
        var dynamicVars = model.GetDynamicVars();
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
                var combatState = model.GetCreature().CombatState;
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

        cmd.Attacker = model.GetCreature();
        cmd.ModelSource = model;

        cmd._attackerAnimName = "Attack";
        cmd._sourceType = AttackCommand.SourceType.Card;
        return cmd;
    }
    
    public static async Task<IEnumerable<CardModel>> Draw(AbstractModel card, PlayerChoiceContext context)
    {
        var player = card.GetCreature().Player;
        if (player == null) return [];
        return await CardPileCmd.Draw(context, card.GetDynamicVars().Cards.BaseValue, player);
    }
}