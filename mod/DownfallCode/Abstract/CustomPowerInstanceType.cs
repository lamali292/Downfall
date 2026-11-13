using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Abstract;

public interface IInstancedPerTarget
{
    Creature? TargetCreature { get; }
}

public static class CustomPowerInstanceType
{
    [CustomEnum] public static PowerInstanceType InstancedPerTarget;

    internal static readonly Dictionary<PowerInstanceType, Func<PowerModel, Creature, Creature?, PowerModel, bool>>
        PowerInstanceTypes = new();

    private static void RegisterPowerInstanceType(
        PowerInstanceType customType,
        Func<PowerModel, Creature, Creature?, PowerModel, bool> isPowerSame)
    {
        DownfallMainFile.Logger.VeryDebug($"Registered power instance type {customType}");
        PowerInstanceTypes.Add(customType, isPowerSame);
    }

    public static void RegisterAll()
    {
        RegisterPowerInstanceType(InstancedPerTarget,
            (model, _, _, otherPower) => model is IInstancedPerTarget a &&
                                         otherPower is IInstancedPerTarget b &&
                                         a.TargetCreature == b.TargetCreature);
    }
}