using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Interfaces;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;

namespace Snecko.SneckoCode.Vfx;

public partial class NSneckoCharacterSelect : Control, IOverlayScreen
{
    private static readonly Vector2 Slot1Offset = new(-250, 100);
    private static readonly Vector2 Slot2Offset = new(250, 100);
    private bool _animating;

    private Rect2 _bounds1;
    private Rect2 _bounds2;

    private int _controllerIndex = 0;
    private TaskCompletionSource<int>? _selectionTcs;
    private Action<string> _trigger1 = _ => { };
    private Action<string> _trigger2 = _ => { };

    private NCreatureVisuals? _visuals1;
    private NCreatureVisuals? _visuals2;

    public Control? DefaultFocusedControl => null;
    public NetScreenType ScreenType => NetScreenType.None;
    public bool UseSharedBackstop => true;

    public void AfterOverlayOpened() { }
    public void AfterOverlayClosed() { }
    public void AfterOverlayShown() { Visible = true; }
    public void AfterOverlayHidden() { Visible = false; }

    public override void _Ready()
    {
        SetAnchorsPreset(LayoutPreset.FullRect);
        MouseFilter = MouseFilterEnum.Stop;
    }

    public override void _Input(InputEvent @event)
    {
        if (_animating) return;

        // Controller navigation
        if (@event.IsActionPressed(MegaInput.left) || @event.IsActionPressed(MegaInput.right))
        {
            _controllerIndex = _controllerIndex == 0 ? 1 : 0;
            GetViewport().SetInputAsHandled();
            return;
        }

        if (@event.IsActionPressed(MegaInput.select))
        {
            GetViewport().SetInputAsHandled();
            _selectionTcs?.TrySetResult(_controllerIndex);
            return;
        }

        // Mouse
        if (@event is not InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left } mb) return;
        var mousePos = mb.GlobalPosition;
        if (_bounds1 != default && _bounds1.HasPoint(mousePos))
        {
            GetViewport().SetInputAsHandled();
            _selectionTcs?.TrySetResult(0);
        }
        else if (_bounds2 != default && _bounds2.HasPoint(mousePos))
        {
            GetViewport().SetInputAsHandled();
            _selectionTcs?.TrySetResult(1);
        }
    }

    public override void _Process(double delta)
    {
        if (_animating) return;
        var mousePos = GetGlobalMousePosition();

        bool hover1 = (_bounds1 != default && _bounds1.HasPoint(mousePos)) || _controllerIndex == 0;
        bool hover2 = (_bounds2 != default && _bounds2.HasPoint(mousePos)) || _controllerIndex == 1;

        if (_visuals1 != null)
            _visuals1.Scale = hover1 ? new Vector2(1.1f, 1.1f) : new Vector2(1f, 1f);

        if (_visuals2 != null)
            _visuals2.Scale = hover2 ? new Vector2(-1.1f, 1.1f) : new Vector2(-1f, 1f);
    }

    public async Task<int> SelectOne(CharacterModel left, CharacterModel right)
    {
        _visuals1?.QueueFree();
        _visuals2?.QueueFree();
        _bounds1 = default;
        _bounds2 = default;
        _animating = false;
        _controllerIndex = 0;

        var screenCenter = GetViewportRect().Size / 2f;

        _visuals1 = TrySpawnVisuals(left, screenCenter + Slot1Offset, false);
        _visuals2 = TrySpawnVisuals(right, screenCenter + Slot2Offset, true);

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        _trigger1 = _visuals1 != null ? BuildTrigger(left, _visuals1) : _ => { };
        _trigger2 = _visuals2 != null ? BuildTrigger(right, _visuals2) : _ => { };

        _trigger1("Idle");
        _trigger2("Idle");

        if (_visuals1 != null) _bounds1 = GetBoundsRect(_visuals1);
        if (_visuals2 != null) _bounds2 = GetBoundsRect(_visuals2);

        GD.Print($"Presenting {left.Id} vs {right.Id}");

        _selectionTcs = new TaskCompletionSource<int>();
        var chosen = await _selectionTcs.Task;

        GD.Print($"Player chose slot {chosen}");

        _animating = true;
        var winnerVisuals = chosen == 0 ? _visuals1 : _visuals2;
        var winnerTrigger = chosen == 0 ? _trigger1 : _trigger2;
        var loserTrigger = chosen == 0 ? _trigger2 : _trigger1;
        var winnerChar = chosen == 0 ? left : right;

        try
        {
            SfxCmd.Play(winnerChar.AttackSfx);
        }
        catch (Exception e)
        {
            GD.PrintErr($"SFX failed: {e.Message}");
        }

        winnerTrigger("Attack");
        loserTrigger("Hit");

        var waitTime = 1.5f;
        try
        {
            if (winnerVisuals != null && IsInstanceValid(winnerVisuals) && winnerVisuals.SpineBody != null)
            {
                var track = winnerVisuals.SpineAnimation.GetCurrentTrack();
                if (track != null)
                    waitTime = Math.Max(track.GetAnimation().GetDuration(), 1f);
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to get anim length: {e.Message}");
        }

        await Cmd.Wait(waitTime);
        return chosen;
    }

    private NCreatureVisuals? TrySpawnVisuals(CharacterModel character, Vector2 position, bool flipX)
    {
        try
        {
            return SpawnVisuals(character, position, flipX);
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to spawn visuals for {character.Id}: {e.Message}");
            return null;
        }
    }

    private NCreatureVisuals SpawnVisuals(CharacterModel character, Vector2 position, bool flipX)
    {
        var visuals = character.CreateVisuals();
        visuals.Position = position;
        if (flipX)
            visuals.Scale = new Vector2(-1, 1);
        AddChild(visuals);
        return visuals;
    }

    private static Action<string> BuildTrigger(CharacterModel character, NCreatureVisuals visuals)
    {
        switch (visuals)
        {
            case IAnimatedVisuals animated:
                return trigger =>
                {
                    if (!IsInstanceValid(visuals)) return;
                    try
                    {
                        animated.OnAnimationTrigger(trigger);
                    }
                    catch (Exception e)
                    {
                        GD.PrintErr($"OnAnimationTrigger {trigger} failed: {e.Message}");
                    }
                };

            case { HasSpineAnimation: true, SpineBody: not null }:
            {
                CreatureAnimator? animator = null;
                try
                {
                    if (character is CustomCharacterModel custom)
                        animator = custom.SetupCustomAnimationStates(visuals.SpineBody);
                    animator ??= character.GenerateAnimator(visuals.SpineBody);
                }
                catch (Exception e)
                {
                    GD.PrintErr($"Animator setup failed for {character.Id}: {e.Message}");
                    return _ => { };
                }

                return trigger =>
                {
                    if (!IsInstanceValid(visuals) || visuals.SpineBody == null) return;
                    try
                    {
                        animator.SetTrigger(trigger);
                    }
                    catch (Exception e)
                    {
                        GD.PrintErr($"Trigger {trigger} failed: {e.Message}");
                    }
                };
            }

            default:
                return trigger =>
                {
                    if (!IsInstanceValid(visuals)) return;
                    var animName = trigger switch
                    {
                        "Idle" => "idle",
                        "Attack" => "attack",
                        "Cast" => "cast",
                        "Hit" => "hurt",
                        "Dead" => "die",
                        _ => trigger.ToLowerInvariant()
                    };
                    try
                    {
                        CustomAnimation.PlayCustomAnimation(visuals, animName, trigger);
                    }
                    catch (Exception e)
                    {
                        GD.PrintErr($"Custom animation {trigger} failed: {e.Message}");
                    }
                };
        }
    }

    private static Rect2 GetBoundsRect(NCreatureVisuals visuals)
    {
        var bounds = visuals.Bounds;
        var size = bounds.Size * visuals.Scale.Abs();
        var globalPos = bounds.GlobalPosition;

        if (visuals.Scale.X < 0)
            globalPos.X -= size.X;

        var margin = new Vector2(20, 20);
        return new Rect2(globalPos - margin, size + margin * 2);
    }
}