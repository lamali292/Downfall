using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Utils.UI;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Rooms;

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

    private Player? _trackedPlayer;
    private Control? _creatureHitbox;
    public static NSpellbookDisplay Create(Player player)
    {
        var combatRoom = NCombatRoom.Instance;
        var creatureNode = combatRoom?.GetCreatureNode(player.Creature);
        return new NSpellbookDisplay
        {
            _trackedPlayer = player,
            Position = Vector2.Zero,
            _creatureHitbox = creatureNode?.Hitbox
        };
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
        if (_trackedPlayer == null) return;

        foreach (var icon in _iconNodes) icon.QueueFree();
        _iconNodes.Clear();

        foreach (var wrapper in _iconWrappers) wrapper.QueueFree();
        _iconWrappers.Clear();

        var spellbook = AwakenedCmd.GetSpellbookOrThrow(_trackedPlayer);

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
                    Position = new Vector2(4, 4) // Offset slightly from the corner
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

            var reticle = DownfallControllerNav.AttachFocusReticle(wrapper, iconSize / 2f + new Vector2(-1, -3), iconSize, 1f);
            if (reticle != null) wrapper.SetReticle(reticle);
        }

        DownfallControllerNav.WireChain(_iconWrappers, true);
        if (_creatureHitbox != null)
            DownfallControllerNav.LinkAbove(_iconWrappers, _creatureHitbox);
    }

    public override void _Process(double delta)
    {
        if (_trackedPlayer == null || CombatManager.Instance is not { IsInProgress: true }) return;

        _bobTime += (float)delta;
        for (var i = 0; i < _bobOffsets.Length; i++)
            _bobOffsets[i] = Mathf.Sin(_bobTime * _bobSpeeds[i] * Mathf.Pi) * 4f;

        for (var i = 0; i < _iconWrappers.Count; i++)
        {
            var isNext = _iconWrappers[i].CustomMinimumSize.X > IconSize; // Check if it's the "Next" spell
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