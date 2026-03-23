using Downfall.Code.Character.Abstract;
using Godot;

namespace Downfall.Code.Character.Collector;

public sealed class CollectorCardPool : DownfallCardPool
{
    protected override string CharId => Collector.CharacterId;

    public override float H => 0.75f; //Hue; changes the color.
    public override float S => 1f; //Saturation
    public override float V => 1f; //Brightness

    public override Color DeckEntryCardColor => new("ffffff");

    public override bool IsColorless => false;
}