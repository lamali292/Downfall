using Godot;

namespace Hexaghost.HexaghostCode.Vfx;

[GlobalClass]
public partial class NFire : Node2D
{
    public enum FireSize
    {
        Large,
        Small
    }

    private const float LargeScale = 1f;
    private const float SmallScale = 0.5f;

    private const string HueUniform = "HueShift";
    private const string HueTweenPath = "shader_parameter/HueShift";

    private TextureRect? _flame;
    private ShaderMaterial? _flameMaterial;
    private GpuParticles2D? _particles;
    private Tween? _tween;

    private FireColor _currentColor = FireColor.Red;
    public FireSize CurrentSize { get; private set; } = FireSize.Small;

    public override void _Ready()
    {
        _flame = GetNode<TextureRect>("hexaghost_flame");
        _particles = GetNodeOrNull<GpuParticles2D>("hexaghost_flame_particles");
        _flameMaterial = _flame.Material as ShaderMaterial;
        if (_flameMaterial == null) return;

        _flameMaterial = (ShaderMaterial)_flameMaterial.Duplicate();
        _flame.Material = _flameMaterial;

        _flameMaterial.SetShaderParameter(HueUniform, HueFor(_currentColor));
    }

    public void SetState(FireColor color, FireSize size, bool instant = false)
    {
        _currentColor = color;
        CurrentSize = size;

        var targetScale = size == FireSize.Large ? LargeScale : SmallScale;
        
        var displayColor = size == FireSize.Large ? FireColor.Green : color;
        var targetHue = HueFor(displayColor);

        _tween?.Kill();

        if (instant || _flameMaterial == null)
        {
            Scale = new Vector2(targetScale, targetScale);
            _flameMaterial?.SetShaderParameter(HueUniform, targetHue);
            return;
        }

        _tween = CreateTween().SetParallel();

        _tween.TweenProperty(this, "scale", new Vector2(targetScale, targetScale), 0.3f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);

        var currentHue = _flameMaterial.GetShaderParameter(HueUniform).AsSingle();
        var delta = Mathf.Wrap(targetHue - currentHue, -0.5f, 0.5f);
        var tweenTarget = currentHue + delta;

        _tween.TweenProperty(_flameMaterial, HueTweenPath, tweenTarget, 0.3f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);
    }
    
    private static float HueFor(FireColor color) => color switch
    {
        FireColor.Green => 0.0f,
        FireColor.Pink => 0.6f,
        FireColor.Blue => 0.25f,
        FireColor.Red => 0.85f,
        
        
        FireColor.Orange => 0.80f,
        FireColor.Yellow => 0.95f,
        _ => 0.0f
    };
}

public enum FireColor
{
    Red,
    Green,
    Blue,
    Yellow,
    Pink,
    Orange
}

public static class FireColorExtensions
{
    public static Color ToColor(this FireColor fireColor)
    {
        return fireColor switch
        {
            FireColor.Red => new Color(0xFF971BFF),
            FireColor.Green => new Color(0x97FF5FFF),
            FireColor.Blue => new Color(0x70FFFFFF),
            FireColor.Yellow => new Color(1f, 0.9f, 0.1f),
            FireColor.Pink => new Color(0xFF72FFFF),
            FireColor.Orange => new Color(1f, 0.5f, 0.1f),
            _ => Colors.White
        };
    }
}