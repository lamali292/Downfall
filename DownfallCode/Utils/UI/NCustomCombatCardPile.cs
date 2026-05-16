using System.Reflection;
using System.Runtime.CompilerServices;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Downfall.DownfallCode.Utils.UI;

public abstract partial class NCustomCombatCardPile : NCombatCardPile
{
    private static readonly FieldInfo PileField =
        typeof(NCombatCardPile).GetField("_pile", BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly FieldInfo CurrentCountField =
        typeof(NCombatCardPile).GetField("_currentCount", BindingFlags.NonPublic | BindingFlags.Instance)!;

    private Tween? _ownBumpTween;

    private Player? _player;

    protected abstract override PileType Pile { get; }
    public abstract Func<Player, bool> CanUsePile { get; }
    public abstract string ScenePath { get; }
    protected abstract Vector2 HideOffset { get; }

    protected abstract Vector2 HoverTipOffset { get; }
    protected abstract HoverTip BuildHoverTip();
    protected abstract LocString BuildEmptyPileMessage();

    public override void _Ready()
    {
        ConnectSignals();
        _emptyPileMessage = BuildEmptyPileMessage();
    }

    protected override void SetAnimInOutPositions()
    {
        _showPosition = Position;
        _hidePosition = Position + HideOffset;
    }

    public void RefreshAnimPositions()
    {
        _showPosition = Position;
        _hidePosition = Position + HideOffset;
    }

    public override void Initialize(Player player)
    {
        _player = player;

        var pile = Pile.GetPile(player);
        PileField.SetValue(this, pile);

        var countLabel = GetNode<MegaLabel>("CountContainer/Count");

        
        pile.CardAddFinished += () =>
        {
            if (!IsInstanceValid(countLabel)) return;
            UpdateCount(pile.Cards.Count, countLabel);
        };
        pile.CardRemoveFinished += () =>
        {
            if (!IsInstanceValid(countLabel)) return;
            UpdateCount(pile.Cards.Count, countLabel);
        };

        UpdateCount(pile.Cards.Count, countLabel);
    }

    private void UpdateCount(int count, MegaLabel label)
    {
        CurrentCountField.SetValue(this, count);
        label.SetTextAutoSize(count.ToString());
    }

    protected override void OnRelease()
    {
        base.OnRelease();

        if (_player == null || !CombatManager.Instance.IsInProgress) return;

        var pile = Pile.GetPile(_player);
        if (pile.IsEmpty)
        {
            if (NCapstoneContainer.Instance is { InUse: true })
                NCapstoneContainer.Instance.Close();

            var bubble = NThoughtBubbleVfx.Create(
                _emptyPileMessage.GetFormattedText(), _player.Creature, 2.0);
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(bubble);
        }
        else if (NCapstoneContainer.Instance?.CurrentCapstoneScreen is NCardPileScreen screen
                 && screen.Pile == pile)
        {
            NCapstoneContainer.Instance.Close();
        }
        else
        {
            NCardPileScreen.ShowScreen(pile, Hotkeys);
        }
    }

    protected override void OnFocus()
    {
        NHoverTipSet.Remove(this);

        var tip = NHoverTipSet.CreateAndShow(this, BuildHoverTip());
        if (tip != null) tip.GlobalPosition = GlobalPosition + HoverTipOffset;


        _ownBumpTween?.Kill();
        _ownBumpTween = CreateTween();
        _ownBumpTween.TweenProperty(GetNode<Control>("Icon"), "scale",
            Vector2.One * 1.25f, 0.05);
    }

    protected override void OnUnfocus()
    {
        NHoverTipSet.Remove(this);

        _ownBumpTween?.Kill();
        _ownBumpTween = CreateTween().SetParallel();
        _ownBumpTween
            .TweenProperty(GetNode<Control>("Icon"), "scale", Vector2.One, 0.5)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Expo);
        _ownBumpTween
            .TweenProperty(GetNode<Control>("Icon"), "modulate", Colors.White, 0.5)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Expo);
    }
}

internal static class CombatPileButtonRegistry
{
    private static List<Type>? _types;

    internal static IReadOnlyList<Type> Types => _types ??= Discover();

    private static List<Type> Discover()
    {
        var results = new List<Type>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            IEnumerable<Type> types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null)!;
            }

            results.AddRange(types.Where(t =>
                t is { IsClass: true, IsAbstract: false } &&
                t.IsSubclassOf(typeof(NCustomCombatCardPile))));
        }

        return results;
    }

    internal static (string scenePath, Func<Player, bool> canUse) ReadMetadata(Type type)
    {
        var probe = (NCustomCombatCardPile)RuntimeHelpers.GetUninitializedObject(type);
        return (probe.ScenePath, probe.CanUsePile);
    }
}

[HarmonyPatch(typeof(NCombatPilesContainer), nameof(NCombatPilesContainer.Initialize))]
internal class PatchCombatPilesContainer
{
    [HarmonyPostfix]
    private static void AddRegisteredPiles(NCombatPilesContainer __instance, Player player)
    {
        foreach (var type in CombatPileButtonRegistry.Types)
        {
            var (scenePath, canUse) = CombatPileButtonRegistry.ReadMetadata(type);
            if (!canUse(player)) continue;

            var scene = ResourceLoader.Load<PackedScene>(scenePath);
            if (scene == null) continue;

            var button = (NCustomCombatCardPile)scene.Instantiate();
            __instance.AddChildSafely(button);
            button.Initialize(player);

            // Scene anchors/offsets handle positioning — just refresh anim
            // targets after the layout pass has resolved Position.
            var capturedButton = button;
            Callable.From(() => capturedButton.RefreshAnimPositions()).CallDeferred();
        }
    }
}

[HarmonyPatch(typeof(NCombatPilesContainer), nameof(NCombatPilesContainer.AnimIn))]
internal class PatchCombatPilesAnimIn
{
    [HarmonyPostfix]
    private static void AnimInAll(NCombatPilesContainer __instance)
    {
        foreach (var btn in __instance.GetChildren().OfType<NCustomCombatCardPile>())
            btn.AnimIn();
    }
}

[HarmonyPatch(typeof(NCombatPilesContainer), nameof(NCombatPilesContainer.AnimOut))]
internal class PatchCombatPilesAnimOut
{
    [HarmonyPostfix]
    private static void AnimOutAll(NCombatPilesContainer __instance)
    {
        foreach (var btn in __instance.GetChildren().OfType<NCustomCombatCardPile>())
            btn.AnimOut();
    }
}