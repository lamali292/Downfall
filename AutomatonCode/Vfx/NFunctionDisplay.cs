using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Events;
using Automaton.AutomatonCode.Piles;
using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;

namespace Automaton.AutomatonCode.Vfx;

/// <summary>
/// Combat panel listing the Encode and Compile effects of the Function that the local player's
/// Encode pile would currently compile into. Shown only while hovering <see cref="NEncodePile"/>
/// (see <see cref="RevealNextTo"/>/<see cref="HideReveal"/>), positioned right next to it, so it
/// never has a fixed screen spot to fight over with other UI (the multiplayer party list, in
/// particular - see prior history of this file). Local-player-only: a Function's contents aren't
/// public information, so it's only ever created for the player whose own Encode pile it previews.
/// </summary>
public partial class NFunctionDisplay : Control
{
    private const string DisplayScenePath = "res://Automaton/scenes/ui/function_display.tscn";

    /// <summary>Gap kept between the Encode pile icon and this panel's left edge.</summary>
    private const float HoverGap = 24f;

    private static NFunctionDisplay? _instance;

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
    private Tween? _showTween;
    private Control? _followAnchor;
    private bool _hasContent;
    private readonly List<CardModel> _shownSource = new();

    /// <summary>Creates the (initially hidden) display for <paramref name="player"/>'s Encode pile
    /// if one doesn't already exist. No-op for anyone but the local player.</summary>
    public static void EnsureFor(Player player)
    {
        if (!LocalContext.IsMe(player)) return;
        Callable.From(() =>
        {
            if (_instance != null && IsInstanceValid(_instance) && !_instance.IsQueuedForDeletion())
                return;

            var ui = NCombatRoom.Instance?.Ui;
            if (ui == null || !IsInstanceValid(ui)) return;

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

    /// <summary>Shows the local player's Function preview next to <paramref name="anchor"/> (the
    /// hovered Encode pile), or does nothing if there's nothing encoded yet to preview.</summary>
    public static void RevealNextTo(Control anchor)
    {
        var inst = _instance;
        if (inst == null || !IsInstanceValid(inst) || !inst._hasContent) return;
        inst._followAnchor = anchor;
        inst.PositionNextToAnchor();
        inst.FadeIn();
    }

    public static void HideReveal()
    {
        var inst = _instance;
        if (inst == null || !IsInstanceValid(inst)) return;
        inst._followAnchor = null;
        inst.Visible = false;
    }

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Ignore;
        _instance = this;

        _title = GetNode<MegaLabel>("%Title");
        _pips = GetNode<HBoxContainer>("%Pips");
        _pipTemplate = GetNode<TextureRect>("%PipTemplate");
        _pipTemplate.Visible = false;
        _encodePanel = GetNode<Control>("%EncodePanel");
        _encodeText = GetNode<MegaRichTextLabel>("%EncodeText");
        _compilePanel = GetNode<Control>("%CompilePanel");
        _compileText = GetNode<MegaRichTextLabel>("%CompileText");
        _layout = GetNode<Control>("Layout");
        Visible = false;

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
        Refresh();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (_instance == this) _instance = null;
        if (_pile != null)
        {
            _pile.ContentsChanged -= OnPileChanged;
            _pile.CardAddFinished -= OnPileChanged;
            _pile.CardRemoveFinished -= OnPileChanged;
            _pile = null;
        }

        CombatManager.Instance.CombatEnded -= OnCombatEnded;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Visible && _followAnchor != null && IsInstanceValid(_followAnchor))
            PositionNextToAnchor();
    }

    private void PositionNextToAnchor()
    {
        if (_followAnchor == null) return;
        var panelHeight = _layout?.Size.Y ?? 0f;
        GlobalPosition = _followAnchor.GlobalPosition + new Vector2(
            _followAnchor.Size.X + HoverGap,
            _followAnchor.Size.Y * 0.5f - panelHeight * 0.5f);
    }

    private void OnCombatEnded(CombatRoom room)
    {
        if (IsInstanceValid(this) && !IsQueuedForDeletion()) QueueFree();
    }

    private void OnPileChanged() => Refresh();

    private void Refresh()
    {
        if (!IsInstanceValid(this) || IsQueuedForDeletion() || _player == null || _encodeText == null || _compileText == null) return;

        var cards = _pile?.Cards ?? [];
        if (cards.Count == 0)
        {
            _shownSource.Clear();
            _hasContent = false;
            Visible = false;
            return;
        }

        if (_hasContent && cards.SequenceEqual(_shownSource)) return;
        _shownSource.Clear();
        _shownSource.AddRange(cards);

        var fn = CreatePreviewModel(_player, cards);
        if (fn == null)
        {
            _hasContent = false;
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

        _hasContent = true;
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
        Visible = true;
        _showTween = CreateTween();
        _showTween.TweenProperty(this, "modulate:a", 1f, 0.15)
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
