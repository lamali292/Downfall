using BaseLib.Abstracts;
using BaseLib.Hooks;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Events;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.DownfallCode.Powers;

public class SoulBurnPower : DownfallPowerModel, IHasSecondAmount
{
    public SoulBurnPower() : base(PowerType.Debuff)
    {
        WithVar("Turns", 3);
    }

    public string GetSecondAmount()
    {
        return $"{DynamicVars["Turns"].BaseValue}";
    }


    public override IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext ctx)
    {
        if (Amount <= 0) yield break;
        if (DynamicVars["Turns"].BaseValue != 1) yield break;
        yield return new HealthBarForecastSegment(
            Amount,
            new Color("8AD974"),
            HealthBarForecastDirection.FromRight,
            2
        );
    }

    protected override async Task AfterSideTurnStart(PlayerChoiceContext ctx, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side)
            return;
        DynamicVars["Turns"].UpgradeValueBy(-1);
        InvokeDisplayAmountChanged();
        if (DynamicVars["Turns"].BaseValue > 0) return;
        await Detonate(ctx, Applier);
    }

    public async Task Detonate(PlayerChoiceContext ctx, Creature? applier = null, bool keepOne = false)
    {
        if (Owner.CombatState == null) return;
        var combatState = Owner.CombatState;
        var owner = Owner;
        var targetAll = await DownfallHook.ShouldSoulburnDetonateTargetAll(Owner.CombatState, ctx, Owner);
        if (targetAll)
            await CreatureCmd.Damage(ctx, CombatState.HittableEnemies, keepOne ? Amount - 1 : Amount,
                ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        else
            await CreatureCmd.Damage(ctx, Owner, keepOne ? Amount - 1 : Amount,
                ValueProp.Unblockable | ValueProp.Unpowered, null, null);

        if (keepOne)
            await PowerCmd.ModifyAmount(ctx, this, 1 - Amount, applier, null);
        else
            await PowerCmd.Remove(this);
        await DownfallHook.AfterSoulburnDetonate(combatState, ctx, owner);
        await Cmd.CustomScaledWait(0.1f, 0.25f);
    }
}