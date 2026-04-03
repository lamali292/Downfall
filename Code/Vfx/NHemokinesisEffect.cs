using Godot;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Downfall.Code.Vfx;

public partial class NHemokinesisEffect : Node
{
    private float _duration = 0.5f;
    private float _spawnInterval = 0.04f;
    private Vector2 _startPos;
    private Vector2 _targetPos;
    private float _timer;

    public static void Spawn(Vector2 start, Vector2 target)
    {
        var manager = new NHemokinesisEffect
        {
            _startPos = start,
            _targetPos = target
        };

        NCombatRoom.Instance?.CombatVfxContainer.AddChild(manager);
    }

    public override void _Process(double delta)
    {
        var fDelta = (float)delta;
        _timer += fDelta;
        if (_duration > 0)
        {
            _duration -= fDelta;
            if (!(_timer >= _spawnInterval)) return;
            _timer = 0f;
            var particle = NHemokinesisParticle.Create(_startPos, _targetPos);
            AddChild(particle);
        }
        else
        {
            if (GetChildCount() == 0) QueueFree();
        }
    }
}