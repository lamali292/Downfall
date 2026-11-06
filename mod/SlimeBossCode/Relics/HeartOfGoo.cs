using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Events;

namespace SlimeBoss.SlimeBossCode.Relics;

[Pool(typeof(SlimeBossRelicPool))]
public class HeartOfGoo : SlimeBossRelicModel, IAfterConsumeEffect
{
    public HeartOfGoo() : base(RelicRarity.Starter)
    {
        WithHeal(2);
        WithVar("UsesLeft", 8);
        WithVar("MaxUses", 8);
    }

    private DynamicVar UsesLeft => DynamicVars["UsesLeft"];
    private DynamicVar MaxUses => DynamicVars["MaxUses"];

    public override int DisplayAmount => UsesLeft.IntValue;
    public override bool ShowCounter => CombatManager.Instance.IsInProgress;


    public async Task AfterConsumeEffect(PlayerChoiceContext ctx, Creature creature, Creature attacker,
        decimal goopAmount)
    {
        if (attacker != Owner.Creature && UsesLeft.BaseValue > 0) return;
        var heal = Math.Min(DynamicVars.Heal.BaseValue, UsesLeft.BaseValue);
        if (heal <= 0) return;
        await CreatureCmd.Heal(Owner.Creature, heal);
        UsesLeft.BaseValue -= heal;
        Flash();
        InvokeDisplayAmountChanged();
    }

    public override RelicModel GetUpgradeReplacement()
    {
        return ModelDb.Relic<BlackHeartOfGoo>();
    }

    public override Task BeforeCombatStart()
    {
        UsesLeft.BaseValue = MaxUses.BaseValue;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}