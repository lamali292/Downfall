namespace Downfall.Code.Vfx;

using Godot;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using System.Collections.Generic;

public partial class NHemokinesisEffect : Node
{
    private Vector2 _startPos;
    private Vector2 _targetPos;
    private float _timer = 0f;
    private float _spawnInterval = 0.04f; // Spawns a particle every 0.04s
    private float _duration = 0.5f;

    public static void Spawn(Vector2 start, Vector2 target)
    {
        var manager = new NHemokinesisEffect 
        { 
            _startPos = start, 
            _targetPos = target 
        };
        
        // Add it to the combat VFX layer so it renders over the characters
        NCombatRoom.Instance?.CombatVfxContainer.AddChild(manager);
    }

    public override void _Process(double delta)
    {
        var fDelta = (float)delta;
        _timer += fDelta;
    
        // 1. Only spawn if we still have duration left
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
            if (GetChildCount() == 0) 
            {
                QueueFree();
            }
        }
    }
}