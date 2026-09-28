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
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;

[HarmonyPatch(typeof(NMultiplayerPlayerState))]
public static class NMultiplayerPlayerStatePatch
{
    private static readonly ConditionalWeakTable<NMultiplayerPlayerState, MyStatState> State = new();

    private class MyStatState
    {
        public required Control Container;
        public required MegaLabel Label;
        public Action<PlayerCombatState, int>? Handler;
        public Action<CombatState>? CombatSetUpHandler;
        public Action<CombatRoom>? CombatEndedHandler;
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

        // player.PlayerCombatState is never nulled between combats (only replaced wholesale by the
        // next ResetCombatState()), so its last Reserve value - and thus our icon - would otherwise
        // keep showing after combat ends, unlike the vanilla energy/star/card containers this is
        // modeled on, which explicitly hide themselves on CombatEnded (NMultiplayerPlayerState.OnCombatEnded).
        state.CombatSetUpHandler = _ => RefreshMyValue(__instance, state, player.PlayerCombatState?.Reserve ?? 0);
        state.CombatEndedHandler = _ => state.Container.Visible = false;
        CombatManager.Instance.CombatSetUp += state.CombatSetUpHandler;
        CombatManager.Instance.CombatEnded += state.CombatEndedHandler;

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
        if (State.TryGetValue(__instance, out var state))
        {
            if (state.Handler != null)
            {
                var reserveResource = CardResourceRegistry.Get<CollectorEnergy>();
                if (reserveResource != null)
                    reserveResource.Changed -= state.Handler;
            }

            if (state.CombatSetUpHandler != null)
                CombatManager.Instance.CombatSetUp -= state.CombatSetUpHandler;
            if (state.CombatEndedHandler != null)
                CombatManager.Instance.CombatEnded -= state.CombatEndedHandler;
        }

        State.Remove(__instance);
    }

    private static string GetLabelNodeName(Control duplicatedContainer)
    {
        return "StarCount";
    }
}