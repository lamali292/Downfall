using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Events;
using Automaton.AutomatonCode.Piles;
using BaseLib.Config;
using Downfall.DownfallCode.Config;
using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;

namespace Automaton.AutomatonCode.Vfx;

/// <summary>
/// Fixed-position combat panel listing the Encode and Compile effects of the Function that a
/// player's Encode pile would currently compile into. One panel per player (any character, local or
/// remote), created when that player encodes a card and freed once their Encode pile is empty again.
/// </summary>
public partial class NFunctionDisplay : Control
{
    private const string DisplayScenePath = "res://Automaton/scenes/ui/function_display.tscn";

    /// <summary>Top-left corner of the first panel, in combat-UI coordinates.</summary>
    private static readonly Vector2 FixedPosition = new(30f, 145f);

    /// <summary>Horizontal distance between the panels of different players.</summary>
    private const float PlayerColumnSpacing = 340f;

    /// <summary>How long the drawer slide takes, in seconds.</summary>
    private const float DrawerSlideDuration = 0.25f;

    /// <summary>Points right, in the direction the drawer opens.</summary>
    private const string RightArrowTexturePath = "res://images/packed/common_ui/settings_tiny_right_arrow.png";

    /// <summary>Points left, in the direction the drawer closes.</summary>
    private const string LeftArrowTexturePath = "res://images/packed/common_ui/settings_tiny_left_arrow.png";

    private const float ArrowValueDefault = 0.9f;
    private const float ArrowValueHovered = 1.2f;

    private Player? _player;
    private CardPile? _pile;
    private MegaLabel? _title;
    private HBoxContainer? _pips;
    private TextureRect? _pipTemplate;
    private Control? _encodePanel;
    private MegaRichTextLabel? _encodeText;
    private Control? _compilePanel;
    private MegaRichTextLabel? _compileText;
    private Control? _layout;
    private TextureButton? _toggleButton;
    private ShaderMaterial? _toggleShader;
    private Tween? _showTween;
    private Tween? _drawerTween;
    private Tween? _toggleHoverTween;
    private float _openX;
    private bool _isOpen = true;
    private readonly List<CardModel> _shownSource = new();

    public static void ShowFor(Player player)
    {
        Callable.From(() =>
        {
            var ui = NCombatRoom.Instance?.Ui;
            if (ui == null || !IsInstanceValid(ui)) return;
            if (ui.GetChildren().OfType<NFunctionDisplay>().Any(d => d._player == player && !d.IsQueuedForDeletion()))
                return;

            var scene = ResourceLoader.Load<PackedScene>(DisplayScenePath);
            if (scene == null)
            {
                GD.PrintErr($"[Automaton] Could not load {DisplayScenePath}");
                return;
            }

            var display = scene.Instantiate<NFunctionDisplay>();
            display._player = player;
            ui.AddChildSafely(display);
        }).CallDeferred();
    }

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Ignore;
        Position = FixedPosition + new Vector2(PlayerColumnSpacing * PlayerIndex(), 0f);
        _openX = Position.X;

        _title = GetNode<MegaLabel>("%Title");
        _pips = GetNode<HBoxContainer>("%Pips");
        _pipTemplate = GetNode<TextureRect>("%PipTemplate");
        _pipTemplate.Visible = false;
        _encodePanel = GetNode<Control>("%EncodePanel");
        _encodeText = GetNode<MegaRichTextLabel>("%EncodeText");
        _compilePanel = GetNode<Control>("%CompilePanel");
        _compileText = GetNode<MegaRichTextLabel>("%CompileText");
        _layout = GetNode<Control>("Layout");
        _toggleButton = GetNode<TextureButton>("%ToggleButton");
        _toggleShader = _toggleButton.Material as ShaderMaterial;
        _toggleButton.Pressed += OnToggleButtonPressed;
        _toggleButton.MouseEntered += OnToggleButtonHoverStart;
        _toggleButton.MouseExited += OnToggleButtonHoverEnd;

        _isOpen = DownfallConfig.AutomatonFunctionDisplayOpen;
        Position = new Vector2(_isOpen ? _openX : ClosedX(), Position.Y);
        ApplyToggleAppearance();

        GetNode<MegaLabel>("%EncodeTitle").SetTextAutoSize(
            new LocString("static_hover_tips", "AUTOMATON-ENCODE.title").GetFormattedText());
        GetNode<MegaLabel>("%CompileTitle").SetTextAutoSize(
            new LocString("static_hover_tips", "AUTOMATON-COMPILE.title").GetFormattedText());

        if (_player != null)
        {
            _pile = EncodePile.FunctionSequence.GetPile(_player);
            _pile.ContentsChanged += OnPileChanged;
            _pile.CardAddFinished += OnPileChanged;
            _pile.CardRemoveFinished += OnPileChanged;
        }

        CombatManager.Instance.CombatEnded += OnCombatEnded;
        // The pile is usually still empty here (ShowFor runs before the card is added): stay hidden, don't free.
        Refresh(freeWhenEmpty: false);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (_pile != null)
        {
            _pile.ContentsChanged -= OnPileChanged;
            _pile.CardAddFinished -= OnPileChanged;
            _pile.CardRemoveFinished -= OnPileChanged;
            _pile = null;
        }

        CombatManager.Instance.CombatEnded -= OnCombatEnded;
        if (_toggleButton != null)
        {
            _toggleButton.Pressed -= OnToggleButtonPressed;
            _toggleButton.MouseEntered -= OnToggleButtonHoverStart;
            _toggleButton.MouseExited -= OnToggleButtonHoverEnd;
        }
    }

    private float ClosedX() => _openX - (_layout?.Size.X ?? 0f);

    private void OnToggleButtonPressed()
    {
        _isOpen = !_isOpen;
        DownfallConfig.AutomatonFunctionDisplayOpen = _isOpen;
        ModConfig.SaveDebounced<DownfallConfig>();

        var targetX = _isOpen ? _openX : ClosedX();
        _drawerTween?.Kill();
        _drawerTween = CreateTween();
        _drawerTween.TweenProperty(this, "position:x", targetX, DrawerSlideDuration)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);

        ApplyToggleAppearance();
    }

    /// <summary>Arrow points in the direction the button will move the drawer: left (close) while open,
    /// right (open) while closed.</summary>
    private void ApplyToggleAppearance()
    {
        if (_toggleButton == null) return;
        var path = _isOpen ? LeftArrowTexturePath : RightArrowTexturePath;
        _toggleButton.TextureNormal = ResourceLoader.Load<Texture2D>(path);
    }

    private void OnToggleButtonHoverStart()
    {
        _toggleHoverTween?.Kill();
        _toggleShader?.SetShaderParameter("v", ArrowValueHovered);
        _toggleButton!.Scale = Vector2.One * 1.1f;
    }

    private void OnToggleButtonHoverEnd()
    {
        if (_toggleShader == null) return;
        _toggleHoverTween?.Kill();
        _toggleHoverTween = CreateTween().SetParallel();
        _toggleHoverTween.TweenMethod(Callable.From<float>(v => _toggleShader.SetShaderParameter("v", v)),
            ArrowValueHovered, ArrowValueDefault, 0.5).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
        _toggleHoverTween.TweenProperty(_toggleButton, "scale", Vector2.One, 0.5)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
    }

    private void OnCombatEnded(CombatRoom room)
    {
        if (IsInstanceValid(this) && !IsQueuedForDeletion()) QueueFree();
    }

    private int PlayerIndex()
    {
        var players = CombatManager.Instance.DebugOnlyGetState()?.Players;
        if (players == null || _player == null) return 0;
        var index = players.ToList().IndexOf(_player);
        return index < 0 ? 0 : index;
    }

    private void OnPileChanged() => Refresh(freeWhenEmpty: true);

    private void Refresh(bool freeWhenEmpty)
    {
        if (!IsInstanceValid(this) || IsQueuedForDeletion() || _player == null || _encodeText == null || _compileText == null) return;

        var cards = _pile?.Cards ?? [];
        if (cards.Count == 0)
        {
            _shownSource.Clear();
            Visible = false;
            if (freeWhenEmpty) QueueFree();
            return;
        }

        if (Visible && cards.SequenceEqual(_shownSource)) return;
        _shownSource.Clear();
        _shownSource.AddRange(cards);

        var fn = CreatePreviewModel(_player, cards);
        if (fn == null)
        {
            Visible = false;
            return;
        }

        _title?.SetTextAutoSize(fn.Title);
        RefreshPips(cards.Count, AutomatonCmd.GetMax(_player));

        _encodeText.Text = JoinLines(fn.GetEncodeLines());
        var compile = JoinLines(fn.GetCompileLines());
        _compileText.Text = compile;
        if (_compilePanel != null) _compilePanel.Visible = compile.Length > 0;
        if (_encodePanel != null) _encodePanel.Visible = _encodeText.Text.Length > 0;

        if (!Visible) FadeIn();
        Visible = true;
    }

    /// <summary>One pip per Encode slot; filled pips are the cards already in the pile.</summary>
    private void RefreshPips(int filled, int max)
    {
        if (_pips == null || _pipTemplate == null) return;
        foreach (var child in _pips.GetChildren().OfType<TextureRect>().Where(c => c != _pipTemplate))
            child.QueueFree();

        for (var i = 0; i < max; i++)
        {
            var pip = (TextureRect)_pipTemplate.Duplicate();
            pip.Visible = true;
            pip.Modulate = i < filled ? Colors.White : new Color(1f, 1f, 1f, 0.3f);
            _pips.AddChild(pip);
        }
    }

    private void FadeIn()
    {
        _showTween?.Kill();
        Modulate = new Color(Modulate, 0f);
        _showTween = CreateTween();
        _showTween.TweenProperty(this, "modulate:a", 1f, 0.25)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
    }

    private static string JoinLines(IEnumerable<string> lines)
    {
        return string.Join("\n", lines.Where(l => !string.IsNullOrWhiteSpace(l)));
    }

    private static FunctionCard? CreatePreviewModel(Player player, IReadOnlyList<CardModel> sourceCards)
    {
        if (ModelDb.Card<FunctionCard>().ToMutable() is not FunctionCard model) return null;
        model.SetSourceCards(sourceCards);
        model.Owner = player;
        return AutomatonHook.ModifyCompiledFunction(player.Creature.CombatState!, model, player, out _);
    }

}
