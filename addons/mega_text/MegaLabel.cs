using System;
using Godot;
using MegaCrit.Sts2.Core.Localization.Fonts;

namespace MegaCrit.Sts2.addons.mega_text;

[Tool]
public partial class MegaLabel : Label
{
	private static readonly TextParagraph _cachedParagraph = new TextParagraph();

	private const float _sizeComparisonEpsilon = 0.01f;

	private bool _autoSizeEnabled = true;

	private int _minFontSize = 8;

	private int _maxFontSize = 100;

	private int _lastSetSize;

	private Vector2 _lastAdjustedSize;

	[Export(PropertyHint.None, "")]
	public bool AutoSizeEnabled
	{
		get
		{
			return _autoSizeEnabled;
		}
		set
		{
			if (_autoSizeEnabled != value)
			{
				_autoSizeEnabled = value;
				if (Engine.IsEditorHint())
				{
					AdjustFontSize();
				}
			}
		}
	}

	[Export(PropertyHint.None, "")]
	public int MinFontSize
	{
		get
		{
			return _minFontSize;
		}
		set
		{
			if (_minFontSize != value)
			{
				_minFontSize = value;
				if (Engine.IsEditorHint())
				{
					AdjustFontSize();
				}
			}
		}
	}

	[Export(PropertyHint.None, "")]
	public int MaxFontSize
	{
		get
		{
			return _maxFontSize;
		}
		set
		{
			if (_maxFontSize != value)
			{
				_maxFontSize = value;
				if (Engine.IsEditorHint())
				{
					AdjustFontSize();
				}
			}
		}
	}

	public override void _Ready()
	{
		MegaLabelHelper.AssertThemeFontOverride(this, ThemeConstants.Label.font);
		RefreshFont();
		AdjustFontSize();
	}

	public void RefreshFont()
	{
		this.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
	}

	public override void _Notification(int what)
	{
		if ((long)what == 40 && !(_lastAdjustedSize.DistanceSquaredTo(base.Size) < 0.0001f))
		{
			AdjustFontSize();
		}
	}

	public void SetTextAutoSize(string text)
	{
		if (!(base.Text == text))
		{
			base.Text = text;
			AdjustFontSize();
		}
	}

	private void SetFontSize(int size)
	{
		if (_lastSetSize != size)
		{
			_lastSetSize = size;
			if (HasThemeFont(ThemeConstants.Label.font))
			{
				AddThemeFontSizeOverride(ThemeConstants.Label.fontSize, size);
			}
		}
	}

	private void AdjustFontSize()
	{
		if (!AutoSizeEnabled)
		{
			return;
		}
		_lastAdjustedSize = base.Size;
		Font themeFont = GetThemeFont(ThemeConstants.Label.font, "Label");
		float lineSpacing = GetThemeConstant(ThemeConstants.Label.lineSpacing, "Label");
		Vector2 size = GetRect().Size;
		bool wrap = base.AutowrapMode != TextServer.AutowrapMode.Off;
		if (!MegaLabelHelper.IsTooBig(_cachedParagraph, base.Text, themeFont, MaxFontSize, lineSpacing, wrap, size))
		{
			SetFontSize(MaxFontSize);
			return;
		}
		if (_lastSetSize >= MinFontSize && _lastSetSize < MaxFontSize && !MegaLabelHelper.IsTooBig(_cachedParagraph, base.Text, themeFont, _lastSetSize, lineSpacing, wrap, size) && MegaLabelHelper.IsTooBig(_cachedParagraph, base.Text, themeFont, _lastSetSize + 1, lineSpacing, wrap, size))
		{
			SetFontSize(_lastSetSize);
			return;
		}
		int num = MinFontSize;
		int num2 = MaxFontSize;
		while (num2 >= num)
		{
			int num3 = num + (num2 - num) / 2;
			if (num3 == MaxFontSize || MegaLabelHelper.IsTooBig(_cachedParagraph, base.Text, themeFont, num3, lineSpacing, wrap, size))
			{
				num2 = num3 - 1;
			}
			else
			{
				num = num3 + 1;
			}
		}
		SetFontSize(Math.Min(num, num2));
	}
}
