using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

namespace Downfall.Code.Vfx;

using Godot;
using System.Threading.Tasks;

public partial class NShockWaveVfx : Node2D
{
    private Color _color;
    private int _count = 40;

    // No more ScenePath or PreloadManager.Cache.GetScene needed
    public static NShockWaveVfx Create(Vector2 position, Color color)
    {
        var vfx = new NShockWaveVfx(); // Standard C# instantiation
        vfx.GlobalPosition = position;
        vfx._color = color;
        return vfx;
    }

    public override void _Ready()
    {
        // Use Godot's built-in Task runner or your TaskHelper
        _ = PlaySequence(); 
    }

    private async Task PlaySequence()
    {
        NGame.Instance?.ScreenShake(ShakeStrength.Strong, ShakeDuration.Short);
        SpawnBurst(_color);
        await ToSignal(GetTree().CreateTimer(2.2f), SceneTreeTimer.SignalName.Timeout);
        this.QueueFree();
    }

    private void SpawnBurst(Color color)
    {
        for (var i = 0; i < _count; i++)
        {
            var speed = (float)GD.RandRange(1000.0f, 1300.0f);
            var delay = (float)GD.RandRange(0.0f, 0.2f);

            // Use the new sceneless factory
            var p = NBlurWaveParticle.Create();
            AddChild(p); 
            p.Setup(color, speed, delay);
        }
    }
}