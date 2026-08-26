using Downfall.DownfallCode.Nodes;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Automaton.AutomatonCode.Vfx;

/// <summary>
///     Generic hover-reveal card slot display: a preview slot that, on hover, fans out
///     a row of card slots to one side. Knows nothing about piles, players, or game rules —
///     subclasses supply the data via the abstract/virtual members.
/// </summary>
public abstract partial class NSlotRevealDisplay : Control
{
    public enum RevealDirection
    {
        Left,
        Right
    }

    private readonly float[] _bobSpeeds = [1.1f, 0.9f, 1.05f, 0.95f];
    private readonly float[] _lastBobOffsets = new float[4];
    private readonly Vector2[] _slotHomes = new Vector2[4];

    // --- State ---
    protected readonly List<NCustomCardHolder> CardHolders = [];
    protected readonly List<NAutomatonSlot> Slots = [];
    private float _bobTime;
    private Vector2 _hiddenPosition;
    private float _hoverLostTimer;
    private bool _initialized;
    private List<CardModel> _lastDirtyCards = [];
    private float _lastPreviewBob;
    private NCustomCardHolder? _previewHolder;
    private Tween? _revealTween;
    private bool _slotsRevealed;

    // A Refresh that arrived before _Ready built the slots (AddChildSafely can defer the
    // add). Replayed once at the end of _Ready; see the guard in Refresh for why.
    private bool _refreshPending;
    private bool _pendingForce;

    protected Label? CountLabel;
    protected int CurrentMax = 3;
    protected CardModel? PreviewModel;
    protected NAutomatonSlot? PreviewSlot;

    // --- Tunables (override per subclass) ---
    protected virtual float SlotSeparation => 70f;
    protected virtual float PreviewGap => 160f;
    protected virtual float RevealDuration => 0.25f;
    protected virtual float RevealFadeDuration => 0.18f;
    protected virtual float RevealStagger => 0.05f;
    protected virtual float RetractDuration => 0.2f;
    protected virtual float RetractFadeDuration => 0.15f;
    protected virtual float RetractStagger => 0.04f;
    protected virtual float PreviewBobSpeed => 0.85f;
    protected virtual float PreviewBobAmplitude => 15f;
    protected virtual float SlotBobAmplitude => 15f;
    protected virtual float RetractGraceTime => 0.5f;
    protected virtual float PreviewCardScale => 1.5f;

    [Export] public RevealDirection Direction { get; set; } = RevealDirection.Left;

    /// <summary>Whether the display should process hover/bob this frame.</summary>
    protected virtual bool IsActive => true;

    protected bool IsTweenRunning => _revealTween != null && _revealTween.IsValid() && _revealTween.IsRunning();

    // --- Subclass contract ---

    /// <summary>The cards currently occupying the slots, in slot order.</summary>
    protected abstract IReadOnlyList<CardModel> GetSlotCards();

    /// <summary>How many slots are currently usable (≤ number of slot nodes in the scene).</summary>
    protected abstract int GetMaxSlots();

    /// <summary>Model for the preview slot's card. Return null for no preview.</summary>
    protected abstract CardModel? CreatePreviewModel(IReadOnlyList<CardModel> slotCards);

    /// <summary>
    ///     Cards used for the change detection in Refresh. Default: the slot row.
    ///     Override when the preview or count depend on more than the slot cards.
    /// </summary>
    protected virtual IReadOnlyList<CardModel> GetDirtyCheckCards()
    {
        return GetSlotCards();
    }

    /// <summary>Text for the %Count badge. Default: filled/max of the slot row.</summary>
    protected virtual string BuildCountText(IReadOnlyList<CardModel> slotCards)
    {
        return $"{Math.Min(slotCards.Count, CurrentMax)}/{CurrentMax}";
    }

    /// <summary>Called after a card node is placed in a slot. Hook for registration, etc.</summary>
    protected virtual void OnSlotCardSet(int index, CardModel model, NCard node, NCustomCardHolder holder)
    {
    }

    /// <summary>Called for each slot card model right before the slots are cleared. Hook for unregistration.</summary>
    protected virtual void OnSlotCardCleared(CardModel model)
    {
    }

    /// <summary>Called after the preview card node is placed.</summary>
    protected virtual void OnPreviewCardSet(CardModel model, NCard node, NCustomCardHolder holder)
    {
    }

    /// <summary>Cards shown by the inspect screen. Default: slot cards + preview.</summary>
    protected virtual List<CardModel> BuildInspectList()
    {
        var list = CardHolders.Where(h => h.CardModel != null).Select(h => h.CardModel!).ToList();
        if (PreviewModel != null) list.Add(PreviewModel);
        return list;
    }

    public override void _Ready()
    {
        Slots.Add(GetNode<NAutomatonSlot>("%Slot0"));
        Slots.Add(GetNode<NAutomatonSlot>("%Slot1"));
        Slots.Add(GetNode<NAutomatonSlot>("%Slot2"));
        Slots.Add(GetNode<NAutomatonSlot>("%Slot3"));
        PreviewSlot = GetNode<NAutomatonSlot>("%FuncPreview");
        CountLabel = GetNodeOrNull<Label>("%Count");

        // All slots sit centered behind the preview in the scene — that IS the hidden position.
        _hiddenPosition = Slots[0].Position;

        foreach (var slot in Slots)
        {
            slot.Visible = false;
            slot.Modulate = new Color(1f, 1f, 1f, 0f);
        }

        ComputeHomes();

        // Replay a Refresh that arrived before the slots existed (deferred AddChildSafely).
        // Subclass data (_trackedPlayer / _stashPile) is set in SetupFor before the add, so
        // it's available here even though the rest of the subclass _Ready runs after base.
        if (_refreshPending)
        {
            _refreshPending = false;
            var force = _pendingForce;
            _pendingForce = false;
            Refresh(force);
        }
    }

    public NCard? GetNCard(CardModel card)
    {
        var node = CardHolders.Find(h => IsInstanceValid(h) && h.CardModel == card)?.CardNode;
        if (node != null && IsInstanceValid(node) && node.Model == card)
            return node;

        // also check the preview slot — the next-draw card lives there, not in CardHolders
        if (_previewHolder != null && IsInstanceValid(_previewHolder)
                                   && _previewHolder.CardModel == card && _previewHolder.CardNode?.Model == card)
            return _previewHolder.CardNode;

        return null;
    }
    
    public override void _ExitTree()
    {
        // Room teardown / combat end: release everything BEFORE the subtree is freed.
        ReleaseAllSlotCards();
        ReleasePreviewCard();
        _revealTween?.Kill();
        _revealTween = null;
    }

    /// <summary>
    ///     Disposes ONE slot's contents the way the base game does it (see
    ///     NPlayerHand.RemoveCardHolder / OnSelectModeSourceFinished): the holder is
    ///     freed, but the pooled NCard inside it is NEVER freed by us. NCard nodes come
    ///     from NodePool.Get and are reused across the run — the base game detaches the
    ///     card and frees only the holder, letting the card node live on (it may be
    ///     adopted into the hand via FindOnTable). QueueFreeing the NCard ourselves
    ///     destroys a node NPlayerHand/FindOnTablePatch may still reference, which surfaces
    ///     as a disposed node in the hand on the next pile move.
    ///
    ///     ClearCard() already does exactly this: detach the NCard, QueueFree the holder.
    ///     So disposal is just: fire the unregister hook, then ClearCard the slot. We never
    ///     touch the card node.
    /// </summary>
    private static void ReleaseSlot(NAutomatonSlot slot)
    {
        slot.ClearCard();
    }

    protected void ReleaseAllSlotCards()
    {
        // Fire the unregister hook for every model we placed, THEN let each slot detach its
        // card and free its holder. Never QueueFree the NCard — the slot detaches it and the
        // game owns its lifetime from here.
        foreach (var holder in CardHolders)
        {
            var model = holder.CardModel;
            if (model != null)
                OnSlotCardCleared(model);
        }
        CardHolders.Clear();

        foreach (var slot in Slots)
            ReleaseSlot(slot);
    }

    private void ReleasePreviewCard()
    {
        // Preview model is display-only (never registered), so no unregister hook. Its node
        // is a pooled NCard owned by PreviewSlot — detach + free-holder via ClearCard, and
        // leave the card node itself alone.
        if (PreviewSlot != null)
            ReleaseSlot(PreviewSlot);
        _previewHolder = null;
        PreviewModel = null;
    }

    /// <summary>
    ///     Fly-out targets: a row beside the preview (side chosen by <see cref="Direction" />),
    ///     vertically centered on it. The highest-index slot sits closest to the preview.
    /// </summary>
    private void ComputeHomes()
    {
        if (PreviewSlot == null || Slots.Count == 0) return;

        var slotSize = Slots[0].Size;
        var y = PreviewSlot.Position.Y + (PreviewSlot.Size.Y - slotSize.Y) / 2f;

        for (var i = 0; i < Slots.Count && i < _slotHomes.Length; i++)
        {
            var slotsToPreview = CurrentMax - i; // 1 = nearest slot
            float x;
            if (Direction == RevealDirection.Left)
                x = PreviewSlot.Position.X
                    - PreviewGap
                    - slotSize.X
                    - (slotsToPreview - 1) * (slotSize.X + SlotSeparation);
            else
                x = PreviewSlot.Position.X + PreviewSlot.Size.X
                                           + PreviewGap
                                           + (slotsToPreview - 1) * (slotSize.X + SlotSeparation);
            _slotHomes[i] = new Vector2(x, y);
        }
    }

    public Vector2 GetSlotGlobalPosition(int index)
    {
        var clamped = Math.Clamp(index, 0, Math.Max(CurrentMax - 1, 0));
        if (clamped >= Slots.Count) return GlobalPosition;

        var slot = Slots[clamped];
        // Report the home-based anchor; the live Position may be hidden or mid-animation.
        var anchorOffset = slot.CardAnchorGlobal - slot.GlobalPosition;
        return GetGlobalTransform() * _slotHomes[clamped] + anchorOffset;
    }

    public void Refresh(bool force = false)
    {
        // If SetupFor's AddChildSafely deferred our add, _Ready hasn't run yet: Slots is
        // empty and PreviewSlot is null. Running now renders nothing but still sets
        // _initialized and caches _lastDirtyCards, so the next Refresh with unchanged data
        // early-outs and the display stays blank until something changes. Defer and replay
        // from _Ready once the slots are built.
        if (PreviewSlot == null)
        {
            _refreshPending = true;
            _pendingForce |= force;
            return;
        }

        var slotCards = GetSlotCards();
        var dirtyCards = GetDirtyCheckCards();
        var newMax = GetMaxSlots();
        var maxChanged = newMax != CurrentMax;
        if (!force && !maxChanged && _initialized && dirtyCards.SequenceEqual(_lastDirtyCards)) return;
        _lastDirtyCards = dirtyCards.ToList();
        _initialized = true;

        if (maxChanged)
        {
            CurrentMax = newMax;
            ComputeHomes();
        }

        if (CountLabel != null)
            CountLabel.Text = BuildCountText(slotCards);

        RefreshSlots(slotCards);
        RefreshPreview(slotCards);
    }

    private void RefreshSlots(IReadOnlyList<CardModel> slotCards)
    {
        // Unregister hooks + detach/free holders for every slot. ReleaseAllSlotCards already
        // ClearCards every slot, so there is no separate clear loop here (the old double
        // clear detached the card twice and raced the deferred holder free).
        ReleaseAllSlotCards();

        for (var i = 0; i < Slots.Count; i++)
        {
            var slot = Slots[i];
            if (i >= CurrentMax)
            {
                slot.Visible = false;
                continue;
            }

            slot.Visible = _slotsRevealed || IsTweenRunning;

            if (i >= slotCards.Count) continue;

            var cardNode = NCard.Create(slotCards[i]);
            if (cardNode == null) continue;

            var holder = slot.SetCard(cardNode);
            if (holder == null)
            {
                // SetCard couldn't take ownership. Detach + free the holder (if any) via the
                // slot; do NOT QueueFree the card node — the slot/game own its lifetime.
                slot.ClearCard();
                continue;
            }

            holder.SetClickable(true);
            var captured = i;
            holder.Pressed += _ => NGame.Instance?.GetInspectCardScreen().Open(BuildInspectList(), captured);
            cardNode.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
            CardHolders.Add(holder);
            OnSlotCardSet(i, slotCards[i], cardNode, holder);
        }
    }

    private void RefreshPreview(IReadOnlyList<CardModel> slotCards)
    {
        ReleasePreviewCard();

        PreviewModel = CreatePreviewModel(slotCards);
        if (PreviewModel == null || PreviewSlot == null) return;

        var cardNode = NCard.Create(PreviewModel);
        if (cardNode == null)
        {
            PreviewModel = null;
            return;
        }

        _previewHolder = PreviewSlot.SetCard(cardNode, PreviewCardScale);
        if (_previewHolder == null)
        {
            PreviewSlot.ClearCard();
            PreviewModel = null;
            return;
        }

        _previewHolder.SetClickable(true);
        _previewHolder.Pressed += _ =>
        {
            var cards = BuildInspectList();
            NGame.Instance?.GetInspectCardScreen().Open(cards, cards.Count - 1);
        };
        cardNode.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
        OnPreviewCardSet(PreviewModel, cardNode, _previewHolder);
    }

    public override void _Process(double delta)
    {
        if (!IsActive) return;
        UpdateHoverReveal((float)delta);

        _bobTime += (float)delta;
        for (var i = 0; i < _bobSpeeds.Length; i++)
        {
            var newOffset = Mathf.Sin(_bobTime * _bobSpeeds[i] * Mathf.Pi) * SlotBobAmplitude;
            if (!(Mathf.Abs(newOffset - _lastBobOffsets[i]) > 0.05f)) continue;
            _lastBobOffsets[i] = newOffset;
            if (i < Slots.Count)
                Slots[i].BobOffset = newOffset;
        }

        if (PreviewSlot == null) return;
        var previewBob = Mathf.Sin(_bobTime * PreviewBobSpeed * Mathf.Pi) * PreviewBobAmplitude;
        if (!(Mathf.Abs(previewBob - _lastPreviewBob) > 0.05f)) return;
        _lastPreviewBob = previewBob;
        PreviewSlot.BobOffset = previewBob;
    }

    private void UpdateHoverReveal(float delta)
    {
        if (PreviewSlot == null) return;

        var mouse = GetGlobalMousePosition();
        var hovering = PreviewSlot.GetGlobalRect().HasPoint(mouse);

        // Mouse over a revealed slot's home rect also counts as hovering.
        if (!hovering && _slotsRevealed)
            for (var i = 0; i < CurrentMax && i < Slots.Count; i++)
            {
                var slot = Slots[i];
                var rect = new Rect2(GetGlobalTransform() * _slotHomes[i],
                    slot.Size * slot.GetGlobalTransform().Scale);
                if (!rect.HasPoint(mouse)) continue;
                hovering = true;
                break;
            }

        if (hovering)
        {
            _hoverLostTimer = 0f;
            SetSlotsRevealed(true);
            return;
        }

        if (!_slotsRevealed) return;

        _hoverLostTimer += delta;
        if (!(_hoverLostTimer >= RetractGraceTime)) return;
        _hoverLostTimer = 0f;
        SetSlotsRevealed(false);
    }

    protected void SetSlotsRevealed(bool revealed)
    {
        if (_slotsRevealed == revealed) return;
        _slotsRevealed = revealed;

        _revealTween?.Kill();
        _revealTween = null;

        // No usable slots → nothing to animate. Creating a tween anyway spams
        // "Tween started with no Tweeners" errors.
        var animatedSlots = Math.Min(Slots.Count, CurrentMax);
        if (animatedSlots <= 0)
        {
            if (!revealed) OnRetractFinished();
            return;
        }

        _revealTween = CreateTween().SetParallel();

        for (var i = 0; i < animatedSlots; i++)
        {
            var slot = Slots[i];

            if (revealed)
            {
                slot.Visible = true;
                // Snap behind the preview only when starting fully hidden;
                // an interrupted retract continues from where it is.
                if (slot.Modulate.A <= 0.001f)
                    slot.Position = _hiddenPosition;
                var delay = i * RevealStagger;
                _revealTween.TweenProperty(slot, "position", _slotHomes[i], RevealDuration)
                    .SetDelay(delay)
                    .SetTrans(Tween.TransitionType.Cubic)
                    .SetEase(Tween.EaseType.Out);
                _revealTween.TweenProperty(slot, "modulate:a", 1f, RevealFadeDuration)
                    .SetDelay(delay);
            }
            else
            {
                var delay = (CurrentMax - 1 - i) * RetractStagger;
                _revealTween.TweenProperty(slot, "position", _hiddenPosition, RetractDuration)
                    .SetDelay(delay)
                    .SetTrans(Tween.TransitionType.Cubic)
                    .SetEase(Tween.EaseType.In);
                _revealTween.TweenProperty(slot, "modulate:a", 0f, RetractFadeDuration)
                    .SetDelay(delay);
            }
        }

        if (!revealed)
            _revealTween.Chain().TweenCallback(Callable.From(OnRetractFinished));
    }

    private void OnRetractFinished()
    {
        foreach (var slot in Slots)
        {
            slot.Visible = false;
            slot.Position = _hiddenPosition;
        }
    }
}