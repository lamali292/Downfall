using HarmonyLib;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Potions;
using MegaCrit.Sts2.Core.Rooms;

namespace Hermit.HermitCode.Relics;

/// <summary>
///     First 1 times you use a potion each combat, gain a random potion.
///     You can only use 1 potions each combat.
/// </summary>
public sealed class Shotglass : HermitRelicModel
{
    public Shotglass() : base(RelicRarity.Shop)
    {
        WithVar("Limit", 1);
    }
    
    public int AvailableUses { get; set; }
    public bool IsInCombat { get; set; }

    public override bool ShowCounter => IsInCombat;
    public override int DisplayAmount => AvailableUses;


    public override Task BeforeCombatStart()
    {
        AvailableUses = (int)DynamicVars["Limit"].BaseValue;
        IsInCombat = true;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override async Task AfterPotionUsed(PotionModel potion, Creature? target)
    {
        if (potion.Owner != Owner || AvailableUses == 0 || !IsInCombat)
            return;
        AvailableUses--;
        Flash();
        await PotionCmd.TryToProcure(
            PotionFactory.CreateRandomPotionInCombat(Owner, Owner.RunState.Rng.CombatPotionGeneration).ToMutable(),
            Owner);
        InvokeDisplayAmountChanged();

        if (AvailableUses == 0)
            Status = RelicStatus.Disabled;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        IsInCombat = false;
        Status = RelicStatus.Normal;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}



[HarmonyPatch(typeof(NPotionPopup), nameof(NPotionPopup.RefreshButtons))]
internal static class ShotglassLimitPatch
{
    private static void Postfix(NPotionPopup __instance)
    {
        var potion = __instance.Potion;
        var shotglass = potion?.Owner.GetRelic<Shotglass>();
        if (shotglass is { IsInCombat: true, AvailableUses: <= 0 })
            __instance._useButton.Disable();
    }
}