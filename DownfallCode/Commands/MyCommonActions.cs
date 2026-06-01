using BaseLib.Extensions;
using BaseLib.Patches.Features;
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
    public static Task ApplySelf<T>(PlayerChoiceContext ctx, AbstractModel model)
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
        await CreatureCmd.Damage(ctx, target, model.GetDynamicVars().HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered, model.GetCreature(), model as CardModel);
    }

    public static async Task LoseHpToTarget(
        PlayerChoiceContext ctx, AbstractModel model, IEnumerable<Creature> targets)
    {
        await CreatureCmd.Damage(ctx, targets, model.GetDynamicVars().HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered, model.GetCreature(), model as CardModel);
    }

    public static async Task<IReadOnlyList<T>> Apply<T>(
        PlayerChoiceContext ctx, AbstractModel model, Creature? target = null)
        where T : PowerModel
    {
        var creature = model.GetCreature();
        var amount = model.GetDynamicVars().Power<T>().BaseValue;
        var card = model as CardModel;
        var targets = model.MyGetTargets(target).ToList();
        if (targets.Count != 1) return await PowerCmd.Apply<T>(ctx, targets, amount, creature, card);
        var result = await PowerCmd.Apply<T>(ctx, targets[0], amount, creature, card);
        return result is not null ? [result] : [];
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
            return cmd.FromCard(card);
        if (cmd.Attacker != null)
            throw new InvalidOperationException("Attacker has already been set.");

        cmd.Attacker = model.GetCreature();
        cmd.ModelSource = model;

        cmd._attackerAnimName = "Attack";
        cmd._sourceType = AttackCommand.SourceType.Card;
        return cmd;
    }
}