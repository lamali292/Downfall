using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Abstract;

public abstract class DownfallRelicPool<T> : CustomRelicPoolModel
    where T : DownfallCharacterModel
{
    private static T Character => ModelDb.Character<T>();
    
    public override Color LabOutlineColor => Character.LabOutlineColor;
    private static string ModId => Character.ModId!;
    public override string BigEnergyIconPath =>
        $"res://{ModId}/images/character/energy_icon.png";

    public override string TextEnergyIconPath =>
        $"res://{ModId}/images/character/energy_text_icon.png";
}