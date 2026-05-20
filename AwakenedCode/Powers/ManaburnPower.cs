using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Events;
using BaseLib.Patches.Localization;
using Downfall.DownfallCode.Extensions.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Awakened.AwakenedCode.Powers;

public class ManaburnPower : AwakenedPowerModel, IOnDrained, IAddDumbVariablesToPowerDescription
{
    public ManaburnPower() : base(PowerType.Debuff)
    {
        WithTip(AwakenedTip.Drained.WithVars(new EnergyVar(1)));
    }
    
    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        description.Add("IsApplierYou", LocalContext.IsMe(Applier));
    }


    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    public async Task OnDrained(PlayerChoiceContext ctx, Player player, int amount)
    {
        if (Applier != player.Creature || LocalContext.NetId == null) return;
        var originalAmount = Amount * amount;
        var modifiedAmount = AwakenedHook.ModifyManaburnDamage(CombatState, originalAmount, player, out var modifiers);
        await AwakenedHook.AfterModifyingManaburnDamage(CombatState, ctx, player, modifiers);
        await CreatureCmd.Damage(ctx,
            Owner, modifiedAmount,
            ValueProp.Move | ValueProp.Unblockable | ValueProp.Unpowered, player.Creature);
    }
   
}