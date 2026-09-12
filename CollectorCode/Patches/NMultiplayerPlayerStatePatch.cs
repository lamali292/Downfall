using Collector.CollectorCode.Core;
using Collector.CollectorCode.Extensions;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Collector.CollectorCode.Patches;

using System.Runtime.CompilerServices;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;

[HarmonyPatch(typeof(NMultiplayerPlayerState))]
public static class NMultiplayerPlayerStatePatch
{
    private static readonly ConditionalWeakTable<NMultiplayerPlayerState, MyStatState> State = new();

    private class MyStatState
    {
        public required Control Container;
        public required MegaLabel Label;
        public Action<PlayerCombatState, int>? Handler;
    }

    [HarmonyPatch("_Ready")]
    [HarmonyPostfix]
    static void Ready_Postfix(NMultiplayerPlayerState __instance)
    {
        var traverse = Traverse.Create(__instance);
        var topContainer = traverse.Field<HBoxContainer>("_topContainer").Value;
        var starContainer = traverse.Field<Control>("_starContainer").Value;
        var myContainer = (Control)starContainer.Duplicate();
        var player = __instance.Player;

        myContainer.Name = "MyValueContainer";
        myContainer.Visible = false;

        var image = myContainer.GetNode<TextureRect>("Image");
        image.Texture = GD.Load<Texture2D>("res://Collector/images/character/reserve_icon.png");
        var label = myContainer.GetNode<MegaLabel>(GetLabelNodeName(myContainer));

        topContainer.AddChild(myContainer);
        var starIndex = starContainer.GetIndex();
        topContainer.MoveChild(myContainer, starIndex + 1);

        var state = new MyStatState { Container = myContainer, Label = label };
        State.Add(__instance, state);

        var reserveResource = CardResourceRegistry.Get<CollectorEnergy>();
        if (reserveResource != null)
        {
            state.Handler = (combatState, value) =>
            {
                if (combatState == player.PlayerCombatState)
                    RefreshMyValue(__instance, state, value);
            };
            reserveResource.Changed += state.Handler;
        }

        RefreshMyValue(__instance, state, player.PlayerCombatState?.Reserve ?? 0);
    }

    private static void RefreshMyValue(NMultiplayerPlayerState instance, MyStatState state, int value)
    {
        var player = instance.Player;
        var shouldShow = value > 0 && !LocalContext.IsMe(player);

        state.Container.Visible = shouldShow;
        if (shouldShow)
            state.Label.SetTextAutoSize(value.ToString());
    }

    [HarmonyPatch("_ExitTree")]
    [HarmonyPrefix]
    static void ExitTree_Prefix(NMultiplayerPlayerState __instance)
    {
        if (State.TryGetValue(__instance, out var state) && state.Handler != null)
        {
            var reserveResource = CardResourceRegistry.Get<CollectorEnergy>();
            if (reserveResource != null)
                reserveResource.Changed -= state.Handler;
        }
        State.Remove(__instance);
    }

    private static string GetLabelNodeName(Control duplicatedContainer)
    {
        return "StarCount";
    }
}