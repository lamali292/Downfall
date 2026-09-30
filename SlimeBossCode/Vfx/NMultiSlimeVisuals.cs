using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace SlimeBoss.SlimeBossCode.Vfx;

/// <summary>
/// Creature visuals for a slime that stands for several slimes at once (<c>SlimeModel.SlimeAmount</c>).
/// The scene's <c>%Visuals</c> body stays the real one - the game's animator, skin and hit effects keep
/// driving it. <see cref="SetSlimeCount"/> adds count-1 purely cosmetic copies behind it, and
/// <see cref="_Process"/> makes them replay whatever the real body is playing.
/// </summary>
public partial class NMultiSlimeVisuals : NCreatureVisuals
{
    /// <summary>
    /// Layout of the extra slimes relative to the real body, in the order they appear. Slimes 2-3 flank the real
    /// one, then a second row fills in behind (higher and smaller), then a third row caps the pile.
    /// </summary>
    private static readonly (Vector2 Offset, float Scale)[] CloneSlots =
    [
        (new(-35f, -14f), 0.8f), (new(35f, -14f), 0.8f),
        (new(-18f, -42f), 0.7f), (new(18f, -42f), 0.7f),
        (new(-54f, -42f), 0.7f), (new(54f, -42f), 0.7f),
        (new(-18f, -68f), 0.6f), (new(18f, -68f), 0.6f)
    ];

    private const float StaggerSeconds = 0.25f;

    private readonly List<(Node2D Node, MegaSprite Sprite)> _clones = [];
    private string? _mirroredAnimation;

    public int CloneCount => _clones.Count;

    /// <summary>Makes the visuals show <paramref name="count"/> slimes in total (the real body plus clones).</summary>
    public void SetSlimeCount(int count)
    {
        var wanted = Math.Clamp(count - 1, 0, CloneSlots.Length);

        while (_clones.Count > wanted)
        {
            _clones[^1].Node.QueueFree();
            _clones.RemoveAt(_clones.Count - 1);
        }

        while (_clones.Count < wanted)
            AddClone(_clones.Count);

        _mirroredAnimation = null; // force the new clones to pick up the current animation
    }

    private void AddClone(int index)
    {
        var clone = (Node2D)Body.Duplicate();
        clone.UniqueNameInOwner = false;
        var (offset, scale) = CloneSlots[index];
        clone.Position = Body.Position + offset;
        clone.Scale = Body.Scale * scale;
        AddChild(clone);
        MoveChild(clone, 0); // first child draws first: every clone sits behind the real body, later ones further back
        _clones.Add((clone, new MegaSprite(clone)));
    }

    public override void _Process(double delta)
    {
        if (_clones.Count == 0 || SpineBody == null) return;

        var track = SpineBody.GetAnimationState().GetCurrent(0);
        var name = track?.GetAnimationName();
        if (name == null || name == _mirroredAnimation) return;
        _mirroredAnimation = name;

        var loop = track!.IsLoop();
        for (var i = 0; i < _clones.Count; i++)
        {
            var state = _clones[i].Sprite.GetAnimationState();
            state.SetAnimation(name, loop);
            // Looping idles are desynced so the group doesn't move in lockstep; one-shots (hurt, attack) stay in sync.
            if (loop) state.GetCurrent(0)?.SetTrackTime(StaggerSeconds * (i + 1));
        }
    }
}
