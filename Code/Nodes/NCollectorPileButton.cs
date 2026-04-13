using Downfall.Code.Piles;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.addons.mega_text;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Downfall.Code.Nodes;

[GlobalClass]
public partial class NCollectorPileButton : NCombatCardPile
{
    private Player? _player;

    protected override PileType Pile => CollectorPile.Collected;

    public override void _Ready()
    {
        ConnectSignals();
        _emptyPileMessage = new LocString("combat_messages", "OPEN_EMPTY_COLLECTED");
    }

    protected override void SetAnimInOutPositions()
    {
        _showPosition = Position;
        _hidePosition = Position + new Vector2(-160f, 100f);
    }

    public override void Initialize(Player player)
    {
        _player = player;
        var pile = CollectorPile.Collected.GetPile(player);
    
        var pileField = typeof(NCombatCardPile).GetField("_pile", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        pileField?.SetValue(this, pile);

        var hoverTipField = typeof(NCombatCardPile).GetField("_hoverTip",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        hoverTipField?.SetValue(this, new HoverTip(
            new LocString("static_hover_tips", "DOWNFALL-COLLECTED_PILE.title"),
            new LocString("static_hover_tips", "DOWNFALL-COLLECTED_PILE.description")
        ));

        
        var currentCountField = typeof(NCombatCardPile).GetField("_currentCount",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var countLabel = GetNode<MegaLabel>("CountContainer/Count");

        pile.CardAddFinished += () =>
        {
            var count = pile.Cards.Count;
            currentCountField?.SetValue(this, count);
            countLabel.SetTextAutoSize(count.ToString());
        };
        pile.CardRemoveFinished += () =>
        {
            var count = pile.Cards.Count;
            currentCountField?.SetValue(this, count);
            countLabel.SetTextAutoSize(count.ToString());
        };

        var initialCount = pile.Cards.Count;
        currentCountField?.SetValue(this, initialCount);
        countLabel.SetTextAutoSize(initialCount.ToString());
    }

    protected override void OnRelease()
    {
        base.OnRelease(); // handles the bump tween animation
    
        if (_player == null || !CombatManager.Instance.IsInProgress) return;
        var pile = CollectorPile.Collected.GetPile(_player);
        if (pile == null) return;

        if (pile.IsEmpty)
        {
            if (NCapstoneContainer.Instance is { InUse: true })
                NCapstoneContainer.Instance.Close();
            var child = NThoughtBubbleVfx.Create(_emptyPileMessage.GetFormattedText(), _player.Creature, 2.0);
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(child);
        }
        else if (NCapstoneContainer.Instance?.CurrentCapstoneScreen is NCardPileScreen screen && screen.Pile == pile)
            NCapstoneContainer.Instance.Close();
        else
            NCardPileScreen.ShowScreen(pile, Hotkeys);
    }

    public void RefreshAnimPositions()
    {
        _showPosition = Position;
        _hidePosition = Position + new Vector2(-160f, 100f);
    }
    
    protected override void OnFocus()
    {
        // don't call base — it crashes without _hoverTip set
        var bumpTween = CreateTween().SetParallel();
        bumpTween.TweenProperty(GetNode<Control>("Icon"), "scale",
            Vector2.One * 1.25f, 0.05);
    }

    protected override void OnUnfocus()
    {
        var bumpTween = CreateTween().SetParallel();
        bumpTween.TweenProperty(GetNode<Control>("Icon"), "scale",
                Vector2.One, 0.5)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Expo);
    }
}

[HarmonyPatch(typeof(NCombatPilesContainer), nameof(NCombatPilesContainer.Initialize))]
class PatchCombatPilesContainer
{
    [HarmonyPostfix]
    static void AddCollectorPile(NCombatPilesContainer __instance, Player player)
    {
        if (player.Character is not Character.Collector) return;

        var scene = ResourceLoader.Load<PackedScene>("res://Downfall/scenes/ui/collector_pile.tscn");
        if (scene == null) return;

        var button = scene.Instantiate<NCollectorPileButton>();
        __instance.AddChildSafely(button);
        button.Initialize(player);

        var drawPile = __instance.GetNodeOrNull<Control>("%DrawPile");
        if (drawPile == null) return;

        Callable.From(() =>
        {
            button.GlobalPosition = drawPile.GlobalPosition + new Vector2(0, -90f);
            button.RefreshAnimPositions();
        }).CallDeferred();
    }
}

[HarmonyPatch(typeof(NCombatPilesContainer), nameof(NCombatPilesContainer.AnimIn))]
class PatchCombatPilesAnimIn
{
    [HarmonyPostfix]
    static void AnimInCollectorPile(NCombatPilesContainer __instance)
    {
        __instance.GetNodeOrNull<NCollectorPileButton>("CollectorPile")?.AnimIn();
    }
}

[HarmonyPatch(typeof(NCombatPilesContainer), nameof(NCombatPilesContainer.AnimOut))]
class PatchCombatPilesAnimOut
{
    [HarmonyPostfix]
    static void AnimOutCollectorPile(NCombatPilesContainer __instance)
    {
        __instance.GetNodeOrNull<NCollectorPileButton>("CollectorPile")?.AnimOut();
    }
}