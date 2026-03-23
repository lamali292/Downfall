using BaseLib.Abstracts;
using Godot;

namespace Downfall.Code.Character.Abstract;

public abstract class DownfallCardPool : CustomCardPoolModel
{
    protected abstract string CharId { get; }

    public override string Title => CharId;

    public override string? BigEnergyIconPath =>
        $"res://Downfall/character/energy_counters/icon/{CharId.ToSnakeCase()}_energy_icon.png";

    public override string? TextEnergyIconPath =>
        $"res://Downfall/character/energy_counters/text/text_{CharId.ToSnakeCase()}_energy_icon.png";
}