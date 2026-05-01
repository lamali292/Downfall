using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Abstract;

public abstract class DownfallPotionPool<T> : CustomPotionPoolModel
    where T : DownfallCharacterModel
{
    private static T Character => ModelDb.Character<T>();

    public override string EnergyColorName => Character.CharId!;
    public override Color LabOutlineColor => Character.LabOutlineColor;
}