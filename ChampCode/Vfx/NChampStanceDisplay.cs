// NChampStanceDisplay.cs

using BaseLib.Utils;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Stance;
using Downfall.DownfallCode.Utils.UI;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Champ.ChampCode.Vfx;

public partial class NChampStanceDisplay : Control
{
    private const string InactiveChargePath = "res://Champ/images/ui/stance_charge_inactive.png";
    private const int ChargeIconSize = 70;
    private const int Separation = 6;
    private const int IconCount = 3;
    private const int TotalWidth = IconCount * ChargeIconSize + (IconCount - 1) * Separation;
    private const int TotalHeight = ChargeIconSize;
    private const int MarginAboveHead = 20;

    private static readonly Vector2 ReticleCenterOffset = new(ChargeIconSize / 2f, ChargeIconSize / 2f);
    private static readonly Vector2 ReticleVisualSize = new(ChargeIconSize, ChargeIconSize);

    private readonly List<TextureRect> _icons = new();
    private readonly List<StanceIconControl> _wrappers = new();
    private Control? _bounds;
    private Control? _creatureHitbox;
    private Player? _trackedPlayer;

    public static NChampStanceDisplay? Show(Player player)
    {
        var combatRoom = NCombatRoom.Instance;
        var creatureNode = combatRoom?.GetCreatureNode(player.Creature);
        if (creatureNode == null) return null;

        var display = new NChampStanceDisplay
        {
            _trackedPlayer = player,
            _bounds = creatureNode.Visuals.Bounds,
            ZIndex = creatureNode.ZIndex - 1,
            _creatureHitbox = creatureNode.Hitbox
        };

        combatRoom?.CombatVfxContainer.AddChildSafely(display);
        return display;
    }

    public override void _Ready()
    {
        Size = new Vector2(TotalWidth, TotalHeight);

        for (var i = 0; i < IconCount; i++)
        {
            var icon = new TextureRect
            {
                StretchMode = TextureRect.StretchModeEnum.KeepAspect,
                Size = new Vector2(ChargeIconSize, ChargeIconSize),
                MouseFilter = MouseFilterEnum.Ignore
            };

            var wrapper = new StanceIconControl
            {
                Size = new Vector2(ChargeIconSize, ChargeIconSize),
                Position = new Vector2(i * (ChargeIconSize + Separation), 0),
                MouseFilter = MouseFilterEnum.Stop
            };

            wrapper.AddChild(icon);
            AddChild(wrapper);
            _icons.Add(icon);
            _wrappers.Add(wrapper);

            var reticle = DownfallControllerNav.AttachFocusReticle(wrapper, ReticleCenterOffset, ReticleVisualSize, 4f);
            if (reticle != null) wrapper.SetReticle(reticle);
        }

        DownfallControllerNav.WireChain(_wrappers, true);
        if (_creatureHitbox != null)
            // Entry point is the rightmost icon, since that's the "first" stance charge
            DownfallControllerNav.LinkAbove(_wrappers, _creatureHitbox, _wrappers.Count - 1);
        Reposition();
        Refresh();
    }

    private void Reposition()
    {
        if (_bounds == null) return;

        GlobalPosition = new Vector2(
            _bounds.GlobalPosition.X + _bounds.Size.X / 2f - TotalWidth / 2f,
            _bounds.GlobalPosition.Y - TotalHeight - MarginAboveHead
        );
    }

    public void Refresh()
    {
        if (!IsInstanceValid(this)) return;
        if (IsQueuedForDeletion()) return;
        if (_trackedPlayer == null || _icons.Count == 0) return;

        var stance = _trackedPlayer.ChampStance;

        if (stance is ChampNoStance or null)
        {
            QueueFree();
            return;
        }

        var activePath = stance.ChargeIconPath ?? InactiveChargePath;

        for (var i = 0; i < _icons.Count; i++)
        {
            var isActive = i < stance.Charges;
            _icons[i].Texture = ResourceLoader.Load<Texture2D>(isActive ? activePath : InactiveChargePath);
            _wrappers[i].SetTipProvider(() => stance.HoverTip);
        }
    }

    private partial class StanceIconControl : NClickableControl
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