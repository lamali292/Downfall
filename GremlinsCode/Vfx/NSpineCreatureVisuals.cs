using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Gremlins.GremlinsCode.Vfx;

public partial class NSpineCreatureVisuals : NCreatureVisuals
{
    private Vector2 _basePosition;
    private bool _wasFlipped;

    public override void _Ready()
    {
        base._Ready();

        var premultMat = new CanvasItemMaterial
        {
            BlendMode = CanvasItemMaterial.BlendModeEnum.PremultAlpha
        };

        if (SpineBody != null)
            SpineBody.SetNormalMaterial(premultMat);
        else
            GetCurrentBody().Material = premultMat;
        Callable.From(() => _basePosition = _body.Position).CallDeferred();
    }

    public override void _Process(double delta)
    {
        var isFlipped = _body.Scale.X < 0;
        if (isFlipped == _wasFlipped) return;
        _wasFlipped = isFlipped;

        _body.Position = isFlipped
            ? new Vector2(-_basePosition.X, _basePosition.Y)
            : _basePosition;
    }
}