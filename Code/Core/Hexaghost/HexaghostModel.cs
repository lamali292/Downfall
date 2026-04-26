using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.Code.Core.Hexaghost.Ghostflames;
using Downfall.Code.Ghostflames;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using static MegaCrit.Sts2.Core.Entities.Multiplayer.GameActionType;

namespace Downfall.Code.Core.Hexaghost;

public class HexaghostModel() : CustomSingletonModel(true, true)
{
    internal static readonly SpireField<Player, GhostflameModel[]> Wheel = new(StartingWheel);

    internal static readonly SpireField<Player, int> CurrentIndex = new(() => 0);

    private static GhostflameModel[] StartingWheel(Player player)
    {
        return
        [
            DownfallModelDb.Ghostflame<SearingGhostflame>().ToMutable(player),
            DownfallModelDb.Ghostflame<CrushingGhostflame>().ToMutable(player),
            DownfallModelDb.Ghostflame<BolsteringGhostflame>().ToMutable(player),
            DownfallModelDb.Ghostflame<SearingGhostflame>().ToMutable(player),
            DownfallModelDb.Ghostflame<CrushingGhostflame>().ToMutable(player),
            DownfallModelDb.Ghostflame<InfernoGhostflame>().ToMutable(player)
        ];
    }

    public static void ResetWheel(Player player)
    {
        Wheel[player] = StartingWheel(player);
        CurrentIndex[player] = 0;
    }


    public override async Task AfterTurnEnd(PlayerChoiceContext ctx, CombatSide side)
    {
        if (side != CombatSide.Player) return;
        foreach (var player in RunManager.Instance.State?.Players ?? [])
        {
            if (player.Character is not Character.Hexaghost) continue;
            if (HexaghostCmd.GetCurrentFlame(player).IsIgnited)
                await HexaghostCmd.Advance(ctx, player, null, true);
        }
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom) return;
        foreach (var player in RunManager.Instance.State?.Players ?? [])
        {
            if (player.Character is not Character.Hexaghost) continue;
            await HexaghostCmd.ResetWheel(player);
            HexaghostVisualsBridge.Refresh(player);
        }
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (LocalContext.NetId == null) return;
        var ctx = new HookPlayerChoiceContext(
            cardPlay.Card.Owner,
            LocalContext.NetId.Value,
            Combat);
        var retract = cardPlay.Card.Keywords.Contains(DownfallKeywords.Retract);
        if (retract) await HexaghostCmd.Retract(ctx, cardPlay.Card.Owner, cardPlay.Card);
        //var advance = cardPlay.Card.Keywords.Contains(DownfallKeywords.Advance);
        //if (advance) await HexaghostCmd.Advance(ctx, cardPlay.Card.Owner);
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        //var retract = cardPlay.Card.Keywords.Contains(DownfallKeywords.Retract);
        //if (retract) await HexaghostCmd.Retract(ctx, cardPlay.Card.Owner);
        var advance = cardPlay.Card.Keywords.Contains(DownfallKeywords.Advance);
        if (advance) await HexaghostCmd.Advance(ctx, cardPlay.Card.Owner, cardPlay.Card);
    }
}