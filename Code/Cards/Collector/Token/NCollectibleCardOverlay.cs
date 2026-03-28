using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.Code.Cards.Collector.Token;

public partial class NCollectibleCardOverlay : Control
{
    private NCreatureVisuals? _visuals;

    public void SetMonster(MonsterModel monster)
    {
        /*?.QueueFree();
        _visuals = monster.CreateVisuals();
        
        // Scale to fit card portrait area (~300x200px)
        _visuals.Scale = Vector2.One * 0.4f;
        _visuals.Position = new Vector2(150f, 180f); // center of portrait area
        
        AddChild(_visuals);
        
        // Generate animator so spine plays
        var animator = monster.GenerateAnimator(_visuals.SpineBody);
        animator.SetTrigger("Idle");*/
    }
}