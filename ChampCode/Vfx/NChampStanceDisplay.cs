using BaseLib.Utils;
using Champ.ChampCode.Events;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Stance;
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

namespace Champ.ChampCode.Vfx;

public partial class NChampStanceDisplay : NClickableControl
{
    private const string DisplayScenePath = "res://Champ/scenes/ui/stance_display.tscn";

    private Player? _trackedPlayer;

    private TextureProgressBar? _fill;
    private Label? _label;
    public bool IsExiting => _isExiting;
    private NSelectionReticle? _reticle;
    private IEnumerable<IHoverTip>? _tips;
    private Func<IEnumerable<IHoverTip>>? _tipProvider;

    private Vector2 RelativeOffset => new Vector2(-70f, -10f);
    private Vector2 HideOffset => new Vector2(-480f, 128f);

    private Tween? _activeTween;
    private bool _isExiting;

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

    public void AnimIn()
    {
        if (_isExiting) return;

        var targetPos = GetTargetShowPosition();
        var startPos = targetPos + HideOffset;

        _activeTween?.Kill();
        Position = startPos;

        _activeTween = CreateTween();
        _activeTween.TweenProperty(this, "position", targetPos, 0.6f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Expo);
    }

    public void AnimOutAndFree()
    {
        if (_isExiting) return;
        _isExiting = true;

        _activeTween?.Kill();

        var targetPos = Position + HideOffset;

        _activeTween = CreateTween();
        _activeTween.TweenProperty(this, "position", targetPos, 0.5f)
            .SetEase(Tween.EaseType.In)
            .SetTrans(Tween.TransitionType.Back);

        _activeTween.Finished += OnExitAnimFinished;
    }

    private void OnExitAnimFinished()
    {
        if (IsInstanceValid(this) && !IsQueuedForDeletion())
        {
            QueueFree();
        }
    }

    private Vector2 GetTargetShowPosition()
    {
        var ui = NCombatRoom.Instance?.Ui;
        var energyNode = ui?._energyCounter;

        if (energyNode != null && ui != null)
        {
            Vector2 uiLocalPos = energyNode.GlobalPosition - ui.GlobalPosition;
            return uiLocalPos + RelativeOffset;
        }

        return new Vector2(228f, 678f);
    }

    public static NChampStanceDisplay? Show(Player player)
    {
        var combatRoom = NCombatRoom.Instance;
        if (combatRoom?.Ui == null)
            return null;

        var scene = ResourceLoader.Load<PackedScene>(DisplayScenePath);
        if (scene == null)
        {
            GD.PrintErr($"[Champ] Could not load {DisplayScenePath}");
            return null;
        }

        var display = scene.Instantiate<NChampStanceDisplay>();
        display._trackedPlayer = player;

        combatRoom.Ui.AddChildSafely(display);

        return display;
    }

    public override void _Ready()
    {
        ConnectSignals();
        AnchorTop = 1f;
        AnchorBottom = 1f;
        _fill = GetNode<TextureProgressBar>("%Fill");
        _label = GetNode<Label>("Label");

        // Check initial stance before triggering entrance animation
        var initialStance = _trackedPlayer?.ChampStance;
        if (initialStance is ChampNoStance or null)
        {
            QueueFree();
            return;
        }

        AnimIn();
        Refresh();
    }

    public void Refresh()
    {
        if (!IsInstanceValid(this) || IsQueuedForDeletion() || _trackedPlayer == null || _isExiting)
            return;

        var stance = _trackedPlayer.ChampStance;

        if (stance is ChampNoStance or null)
        {
            AnimOutAndFree();
            return;
        }

        // Pointer-equality check skips unnecessary Godot C++ interop setters
        var texProgress = stance.ChargeTextureProgress;
        if (_fill!.TextureProgress != texProgress)
            _fill.TextureProgress = texProgress;

        var texOver = stance.ChargeTextureOver;
        if (_fill.TextureOver != texOver)
            _fill.TextureOver = texOver;

        var texUnder = stance.ChargeTextureUnder;
        if (_fill.TextureUnder != texUnder)
            _fill.TextureUnder = texUnder;

        var maxCharges = stance.MaxCharges;
        var charges = stance.Charges;

        _fill.MaxValue = maxCharges;
        _fill.Value = charges;

        if (stance.LabelOutlineColor is { } color)
            _label!.AddThemeColorOverride("font_outline_color", color);

        if (_trackedPlayer.Creature.CombatState != null &&
            ChampHook.IgnoreChargeCap(_trackedPlayer.Creature.CombatState, _trackedPlayer))
        {
            _label!.Text = "∞";
        }
        else
        {
            _label!.Text = $"{charges}/{maxCharges}";
        }
        

        SetTipProvider(() => stance.HoverTips.Reverse());
    }
    
    public void SetReticle(NSelectionReticle? reticle)
    {
        _reticle = reticle;
    }

    public void SetTipProvider(Func<IEnumerable<IHoverTip>> provider)
    {
        _tipProvider = provider;
    }

    protected override void OnFocus()
    {
        if (NControllerManager.Instance?.IsUsingButtonInputsCompatibility() == true)
            _reticle?.OnSelect();

        _tips = _tipProvider?.Invoke();

        if (_tips == null)
            return;

        var tipSet = NHoverTipSet.CreateAndShow(this, _tips);
        if (tipSet == null)
            return;

        // Position above the display node and anchor to bottom so it expands upward
        float containerHeight = tipSet.TextHoverTipDimensions.Y;
        tipSet.GlobalPosition = GlobalPosition + new Vector2(0f, -containerHeight - 10f);
    }

    protected override void OnUnfocus()
    {
        _reticle?.OnDeselect();
        NHoverTipSet.Remove(this);
    }
}