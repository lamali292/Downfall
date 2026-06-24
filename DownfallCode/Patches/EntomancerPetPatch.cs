namespace Downfall.DownfallCode.Patches;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;





/*
[HarmonyPatch]
internal static class PersonalHivePowerAnyPetPatch
{
    static MethodBase TargetMethod()
    {
        var kickoff = AccessTools.Method(
            typeof(PersonalHivePower),
            nameof(PersonalHivePower.AfterDamageReceived));

        var stateMachine =
            kickoff.GetCustomAttribute<AsyncStateMachineAttribute>()?.StateMachineType
            ?? throw new Exception(
                "[Downfall] PersonalHivePower.AfterDamageReceived is not async; " +
                "the patch target assumption is wrong.");

        return AccessTools.Method(stateMachine, "MoveNext");
    }

    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var petOwnerGetter = AccessTools.PropertyGetter(typeof(Creature), "PetOwner");
        var petOwnerField = petOwnerGetter == null
            ? AccessTools.Field(typeof(Creature), "PetOwner")
            : null;

        var codes = new List<CodeInstruction>(instructions);
        var patched = false;

        for (var i = 0; i < codes.Count; i++)
        {
            if (codes[i].opcode != OpCodes.Isinst) continue;
            if ((codes[i].operand as Type)?.Name != "Osty") continue;
            if (i == 0) continue;

            var load = codes[i - 1];
            if (petOwnerGetter != null)
            {
                load.opcode = OpCodes.Callvirt;
                load.operand = petOwnerGetter;
            }
            else if (petOwnerField != null)
            {
                load.opcode = OpCodes.Ldfld;
                load.operand = petOwnerField;
            }
            else
            {
                Godot.GD.PushError(
                    "[Downfall] PersonalHivePower_AnyPet_Patch: Creature.PetOwner not found; " +
                    "patch not applied.");
                return codes;
            }

            codes[i].opcode = OpCodes.Nop;
            codes[i].operand = null;

            patched = true;
            break;
        }

        if (!patched)
            Godot.GD.PushError(
                "[Downfall] PersonalHivePower_AnyPet_Patch: could not find the `Monster is Osty` " +
                "check in MoveNext. The method may have changed; patch not applied.");

        return codes;
    }
}*/