using BaseLib.Abstracts;
using BaseLib.Hooks;
using Godot;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hexaghost.HexaghostCode.Powers;

public class SoulBurnPower : HexaghostPowerModel, IHasSecondAmount
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

    protected override async Task AfterSideTurnStart(PlayerChoiceContext ctx, CombatSide side, ICombatState combatState)
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
        var targetAll = await HexaghostHook.ShouldSoulburnDetonateTargetAll(Owner.CombatState, ctx, Owner);
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
        await HexaghostHook.AfterSoulburnDetonate(combatState, ctx, owner);
        await Cmd.CustomScaledWait(0.1f, 0.25f);
    }
}