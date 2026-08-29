using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Utils.UI;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;

namespace Awakened.AwakenedCode.Vfx;

public partial class NSpellbookDisplay : Control
{
    private const float IconSize = 64f;
    private const float IconDistance = IconSize + 12f;
    private const int NextSpellBorderWidth = 3;
    private const int NextSpellBorderRadius = 10;
    private const float NextSpellBorderPadding = 2f;
    private static readonly Color NextSpellBorderColor = Colors.Gold;
    private static readonly StyleBoxFlat NextSpellBorderStyle = BuildNextSpellBorderStyle();
    private readonly float[] _bobOffsets = new float[8];
    private readonly float[] _bobSpeeds = [1.1f, 0.9f, 1.05f, 0.95f, 1.0f, 0.85f, 1.15f, 0.98f];

    private readonly List<TextureRect> _iconNodes = [];
    private readonly List<SpellIconControl> _iconWrappers = new();
    private float _bobTime;
    private Control? _creatureHitbox;
    private Tween? _moveTween;
    private Vector2 _restPosition;

    private Player? _trackedPlayer;

    public bool IsExiting { get; private set; }

    private Vector2 RelativeOffset => new(-40f, -650f);
    private Vector2 HideOffset => new(-120f, 0f);

    public override void _EnterTree()
    {
        base._EnterTree();
        CombatManager.Instance.CombatEnded += OnCombatEnded;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        CombatManager.Instance.CombatEnded -= OnCombatEnded;
    }

    private void OnCombatEnded(CombatRoom room)
    {
        AnimOutAndFree();
    }

    public static NSpellbookDisplay? Create(Player player)
    {
        var combatRoom = NCombatRoom.Instance;
        if (combatRoom?.Ui == null)
            return null;

        var creatureNode = combatRoom.GetCreatureNode(player.Creature);
        var display = new NSpellbookDisplay
        {
            _trackedPlayer = player,
            _creatureHitbox = creatureNode?.Hitbox
        };
        combatRoom.Ui.AddChildSafely(display);
        return display;
    }

    public override void _Ready()
    {
        Modulate = new Color(Modulate, 0f); // hidden until positioned
        Refresh(); // build icons now

        var timer = GetTree().CreateTimer(0.7);
        timer.Timeout += () =>
        {
            if (!IsInstanceValid(this)) return;
            _restPosition = GetTargetShowPosition();
            AnimIn();
        };
    }

    private Vector2 GetTargetShowPosition()
    {
        var ui = NCombatRoom.Instance?.Ui;
        var energyNode = ui?._energyCounter;
        if (energyNode == null || ui == null) return Position;
        var localPos = energyNode.GlobalPosition - ui.GlobalPosition;
        return localPos + RelativeOffset;
    }

    private void AnimIn()
    {
        if (IsExiting) return;

        _moveTween?.Kill();

        Position = _restPosition + HideOffset;
        Modulate = new Color(Modulate, 0f);

        _moveTween = CreateTween().SetParallel();
        _moveTween.TweenProperty(this, "position", _restPosition, 0.5f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Expo);
        _moveTween.TweenProperty(this, "modulate:a", 1f, 0.5f)
            .SetEase(Tween.EaseType.Out);
    }

    private void AnimOutAndFree()
    {
        if (IsExiting) return;
        IsExiting = true;

        _moveTween?.Kill();

        var targetPos = Position + HideOffset;

        _moveTween = CreateTween().SetParallel();
        _moveTween.TweenProperty(this, "position", targetPos, 0.4f)
            .SetEase(Tween.EaseType.In)
            .SetTrans(Tween.TransitionType.Back);
        _moveTween.TweenProperty(this, "modulate:a", 0f, 0.4f)
            .SetEase(Tween.EaseType.In);

        _moveTween.Finished += OnExitAnimFinished;
    }

    private void OnExitAnimFinished()
    {
        if (IsInstanceValid(this) && !IsQueuedForDeletion())
            QueueFree();
    }

    private static StyleBoxFlat BuildNextSpellBorderStyle()
    {
        var style = new StyleBoxFlat
        {
            DrawCenter = false,
            BorderColor = NextSpellBorderColor
        };
        style.SetBorderWidthAll(NextSpellBorderWidth);
        style.SetCornerRadiusAll(NextSpellBorderRadius);
        return style;
    }

    public void Refresh()
    {
        if (_trackedPlayer == null || IsExiting) return;
        if (!IsInstanceValid(this) || !IsInsideTree()) return;
        foreach (var wrapper in _iconWrappers.Where(IsInstanceValid)) wrapper.QueueFree();
        _iconWrappers.Clear();
        _iconNodes.Clear();

        var spellbook = AwakenedCmd.GetSpellbook(_trackedPlayer);

        var groupedCards = spellbook.Cards
            .GroupBy(c => c.Id)
            .ToList();

        for (var i = 0; i < groupedCards.Count; i++)
        {
            var group = groupedCards[i];
            var firstCard = group.First();
            var count = group.Count();

            if (firstCard is not ISpell spell) continue;

            var iconPath = spell.SpellIconPath;
            if (!ResourceLoader.Exists(iconPath)) continue;

            var isNext = firstCard == spellbook.NextSpell || group.Contains(spellbook.NextSpell);

            var iconSize = new Vector2(IconSize + (isNext ? 12 : 0), IconSize + (isNext ? 12 : 0));
            var iconPosition = new Vector2(i * IconDistance - (isNext ? 6 : 0), isNext ? -6 : 0);
            var icon = new TextureRect
            {
                Texture = ResourceLoader.Load<Texture2D>(iconPath),
                StretchMode = TextureRect.StretchModeEnum.KeepAspect,
                CustomMinimumSize = iconSize,
                Size = iconSize
            };
            var wrapper = new SpellIconControl
            {
                Size = iconSize,
                CustomMinimumSize = iconSize,
                Position = iconPosition,
                MouseFilter = MouseFilterEnum.Stop
            };

            if (count > 1)
            {
                var label = new Label
                {
                    Text = $"{count}x",
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Size = icon.CustomMinimumSize,
                    Position = new Vector2(4, 4)
                };

                label.AddThemeColorOverride("font_outline_color", Colors.Black);
                label.AddThemeConstantOverride("outline_size", 4);

                icon.AddChild(label);
            }

            wrapper.SetTipProvider(() => HoverTipFactory.FromCard(firstCard));
            wrapper.AddChild(icon);

            if (isNext)
            {
                var padding = new Vector2(NextSpellBorderPadding, NextSpellBorderPadding);
                var nextBorder = new Panel
                {
                    Position = -padding,
                    Size = iconSize + padding * 2f,
                    MouseFilter = MouseFilterEnum.Ignore
                };
                nextBorder.AddThemeStyleboxOverride("panel", NextSpellBorderStyle);
                wrapper.AddChild(nextBorder);
            }

            AddChild(wrapper);
            _iconNodes.Add(icon);
            _iconWrappers.Add(wrapper);

            var reticle =
                DownfallControllerNav.AttachFocusReticle(wrapper, iconSize / 2f + new Vector2(-1, -3), iconSize, 1f);
            if (reticle != null) wrapper.SetReticle(reticle);
        }

        DownfallControllerNav.WireChain(_iconWrappers, true);
        if (_creatureHitbox != null)
            DownfallControllerNav.LinkAbove(_iconWrappers, _creatureHitbox);
    }

    public override void _Process(double delta)
    {
        if (_trackedPlayer == null || IsExiting || CombatManager.Instance is not { IsInProgress: true }) return;

        _bobTime += (float)delta;
        for (var i = 0; i < _bobOffsets.Length; i++)
            _bobOffsets[i] = Mathf.Sin(_bobTime * _bobSpeeds[i] * Mathf.Pi) * 4f;

        for (var i = 0; i < _iconWrappers.Count; i++)
        {
            var isNext = _iconWrappers[i].CustomMinimumSize.X > IconSize;
            _iconWrappers[i].Position = new Vector2(
                i * IconDistance - (isNext ? 6 : 0),
                (i < _bobOffsets.Length ? _bobOffsets[i] : 0f) - (isNext ? 6 : 0)
            );
        }
    }

    private partial class SpellIconControl : NClickableControl
    {
        private NSelectionReticle? _reticle;
        private IHoverTip? _tip;
        private Func<IHoverTip>? _tipProvider;

        public void SetTipProvider(Func<IHoverTip> provider)
        {
            _tipProvider = provider;
        }

        public void SetReticle(NSelectionReticle? reticle)
        {
            _reticle = reticle;
        }

        public override void _Ready()
        {
            ConnectSignals();
        }

        protected override void OnFocus()
        {
            if (NControllerManager.Instance?.IsUsingButtonInputsCompatibility() == true) _reticle?.OnSelect();

            _tip = _tipProvider?.Invoke();
            if (_tip == null) return;
            NHoverTipSet.CreateAndShow(this, _tip)
                ?.SetGlobalPosition(GlobalPosition + new Vector2(0f, Size.Y + 20f));
        }

        protected override void OnUnfocus()
        {
            _reticle?.OnDeselect();
            NHoverTipSet.Remove(this);
        }
    }
}