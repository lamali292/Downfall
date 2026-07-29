using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Compatibility;
using Hexaghost.HexaghostCode.CustomEnums;
using Hexaghost.HexaghostCode.Ghostflames;
using Hexaghost.HexaghostCode.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
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

    public override Task BeforeCombatStart()
    {
        var state = CombatManager.Instance.DebugOnlyGetState();
        if (state == null) return Task.CompletedTask;
        foreach (var player in state.Players)
        {
            ResetWheel(player);
            HexaghostVisualsBridge.Refresh(player);
        }

        return Task.CompletedTask;
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

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)
    {
        if ( card is not IHasAfterlifeEffect afterlifeEffect) return;
        await afterlifeEffect.AfterlifeEffect(ctx, null, true);
    }

    internal static void SetupHexaghostCombatUi(CombatState state)
    {
        if (NCombatRoom.Instance is not { } combatRoom) return;
        foreach (var player in state.Players)
        {
            if (player.Character is not Hexaghost) continue;
            HexaghostVisualsBridge.DiscardDisplay(player);
            HexaghostVisualsBridge.Setup(combatRoom, player);
        }
    }

   
}