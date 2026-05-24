using BaseLib.Abstracts;
using BaseLib.Utils;
using HarmonyLib;
using Hexaghost.HexaghostCode.CustomEnums;
using Hexaghost.HexaghostCode.Ghostflames;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Runs;
using static MegaCrit.Sts2.Core.Entities.Multiplayer.GameActionType;

namespace Hexaghost.HexaghostCode.Core;

public class HexaghostModel() : CustomSingletonModel(HookType.Combat)
{
    internal static readonly SpireField<Player, GhostflameModel[]> Wheel = new(StartingWheel);

    internal static readonly SpireField<Player, int> CurrentIndex = new(() => 0);

    private static GhostflameModel[] StartingWheel(Player player)
    {
        return
        [
            HexaghostModelDb.Ghostflame<SearingGhostflame>().ToMutable(player),
            HexaghostModelDb.Ghostflame<CrushingGhostflame>().ToMutable(player),
            HexaghostModelDb.Ghostflame<BolsteringGhostflame>().ToMutable(player),
            HexaghostModelDb.Ghostflame<SearingGhostflame>().ToMutable(player),
            HexaghostModelDb.Ghostflame<CrushingGhostflame>().ToMutable(player),
            HexaghostModelDb.Ghostflame<InfernoGhostflame>().ToMutable(player)
        ];
    }

    public static void ResetWheel(Player player)
    {
        Wheel[player] = StartingWheel(player);
        CurrentIndex[player] = 0;
    }


    public override async Task BeforeSideTurnEnd(PlayerChoiceContext ctx, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player) return;
        foreach (var player in RunManager.Instance.State?.Players ?? [])
        {
            if (player.Character is not Hexaghost) continue;
            if (HexaghostCmd.GetCurrentFlame(player).IsIgnited)
                await HexaghostCmd.Advance(ctx, player, null, true, true);
        }
    }

    internal static async Task SetupHexaghostCombatUi(CombatState state)
    {
        foreach (var player in state.Players)
        {
            if (player.Character is not Hexaghost) continue;
            await HexaghostCmd.ResetWheel(player);
            HexaghostVisualsBridge.Refresh(player);
            HexaghostCmd.GetCurrentFlame(player).UpdateVisuals();
        }
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        var retract = cardPlay.Card.Keywords.Contains(HexaghostKeyword.Retract);
        if (!retract) return;
        if (LocalContext.NetId == null) return;
        var ctx = new HookPlayerChoiceContext(
            cardPlay.Card.Owner,
            LocalContext.NetId.Value,
            Combat);

        var task = HexaghostCmd.Retract(ctx, cardPlay.Card.Owner, cardPlay.Card);
        await ctx.AssignTaskAndWaitForPauseOrCompletion(task);
        //var advance = cardPlay.Card.Keywords.Contains(HexaghostKeyword.Advance);
        //if (advance) await HexaghostCmd.Advance(ctx, cardPlay.Card.Owner);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        //var retract = cardPlay.Card.Keywords.Contains(HexaghostKeyword.Retract);
        //if (retract) await HexaghostCmd.Retract(ctx, cardPlay.Card.Owner);
        var advance = cardPlay.Card.Keywords.Contains(HexaghostKeyword.Advance);
        if (advance) await HexaghostCmd.Advance(ctx, cardPlay.Card.Owner, cardPlay.Card);
    }
}

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
internal static class HexaghostCombatUiActivatePatch
{
    private static void Postfix(CombatState state)
    {
        _ = HexaghostModel.SetupHexaghostCombatUi(state);
    }
}