using Godot;

namespace Downfall.Code.Vfx;

public partial class NBlurWaveParticle : Sprite2D
{
    private static readonly Texture2D ParticleTexture =
        ResourceLoader.Load<Texture2D>("res://Downfall/vfx/blur_wave.png");

    private readonly Random _rng = new();
    private float _flipper = 90.0f;
    private float _movementRotation;

    public static NBlurWaveParticle Create()
    {
        return new NBlurWaveParticle
        {
            Texture = ParticleTexture,
            Material = new CanvasItemMaterial
            {
                BlendMode = CanvasItemMaterial.BlendModeEnum.Add,
                LightMode = CanvasItemMaterial.LightModeEnum.Unshaded
            }
        };
    }
    
    public void Setup(Color color, float chosenSpeed, float startDelay)
    {
        Material = new CanvasItemMaterial
        {
            BlendMode = CanvasItemMaterial.BlendModeEnum.Add,
            LightMode = CanvasItemMaterial.LightModeEnum.Unshaded
        };

        _movementRotation = (float)_rng.NextDouble() * 360.0f;
        var initialScaleValue = (float)(_rng.NextDouble() * 0.8f + 1.2f);

        RotationDegrees = _movementRotation + _flipper;
        Scale = Vector2.One * initialScaleValue;
        Modulate = new Color(0, 0, 0);
        ZIndex = _rng.Next(0, 2) == 0 ? -1 : 1;

        var direction = Vector2.FromAngle(Mathf.DegToRad(_movementRotation));
        var targetPos = Position + direction * chosenSpeed * 1.2f;

        var tween = CreateTween();
        tween.SetParallel();

        var maxVisibilityColor = new Color(color.R * 0.4f, color.G * 0.4f, color.B * 0.4f);
        var invisibleColor = new Color(0, 0, 0);
        
        tween.TweenProperty(this, "modulate", maxVisibilityColor, 0.4f)
            .SetDelay(startDelay);
        tween.TweenProperty(this, "modulate", invisibleColor, 0.8f)
            .SetDelay(startDelay + 1.4f);
        tween.TweenProperty(this, "position", targetPos, 2.2f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out)
            .SetDelay(startDelay);
        tween.TweenProperty(this, "scale", Scale * 4.5f, 2.2f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out)
            .SetDelay(startDelay);
        tween.SetParallel(false);
        tween.Chain().TweenCallback(Callable.From(QueueFree));
    }
}