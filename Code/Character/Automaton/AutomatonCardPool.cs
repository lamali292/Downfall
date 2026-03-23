using Downfall.Code.Character.Abstract;
using Godot;

namespace Downfall.Code.Character.Automaton;

public sealed class AutomatonCardPool : DownfallCardPool
{
    protected override string CharId => Automaton.CharacterId;

    public override float H => 0.127f; //Hue; changes the color.
    public override float S => 0.52f; //Saturation
    public override float V => 1.0f; //Brightness

    public override Color DeckEntryCardColor => new("ffffff");

    public override bool IsColorless => false;
}