using Godot;

namespace Downfall.DownfallCode.Vfx;

[GlobalClass]
public partial class NStatusPart : Control
{
	private NinePatchRect _hpForeground = null!;
	private NinePatchRect _hpMiddleground = null!;
	private Tween? _foregroundTween;
	private Tween? _showHideTween;

	public override void _Ready()
	{
		_hpForeground = GetNode<NinePatchRect>("ForegroundContainer/Mask/HpForeground");
		_hpMiddleground = GetNode<NinePatchRect>("ForegroundContainer/Mask/HpMiddleground");
	}

	public void Show(bool filled, Color? color = null)
	{
		Callable.From(() => Callable.From(() => Callable.From(() =>
		{
			SetFill(filled, color);
		}).CallDeferred()).CallDeferred()).CallDeferred();
	}
	

	public void SetFill(bool fill,  Color? color = null)
	{
		if (color != null)
			_hpForeground.SelfModulate = color.Value;
		var maxWidth = Size.X ;
		_foregroundTween?.Kill();
		_foregroundTween = CreateTween();
		_hpMiddleground.OffsetRight = _hpForeground.OffsetRight; 
		if (fill)
		{
			_foregroundTween
				.TweenProperty(_hpForeground, "offset_right", 0, 0.3)
				.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
			_foregroundTween
				.TweenProperty(_hpMiddleground, "offset_right", 0, 0.1)
				.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
	
		}
		else
		{
			_foregroundTween
				.TweenProperty(_hpForeground, "offset_right", -maxWidth, 0.3)
				.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
			_foregroundTween
				.TweenProperty(_hpMiddleground, "offset_right", -maxWidth, 0.3)
				.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
		}
	}
}