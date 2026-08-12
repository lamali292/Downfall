using BaseLib.Abstracts;
using BaseLib.Utils;
using Hexaghost.HexaghostCode.Ghostflames;
using Hexaghost.HexaghostCode.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Hexaghost.HexaghostCode.Core;

public class HexaghostModel() : CustomSingletonModel(HookType.Combat)
{
    internal static readonly SpireField<Player, GhostflameModel[]> Wheel = new(StartingWheel);
    internal static readonly SpireField<Player, bool> Active = new(() => false);
    internal static readonly SpireField<Player, int> CurrentIndex = new(() => 0);
    
    
    private static GhostflameModel[] StartingWheel(Player player)
    {
        return player.Character is Hexaghost ?
        [
            HexaghostModelDb.Ghostflame<SearingGhostflame>().ToMutable(player),
            HexaghostModelDb.Ghostflame<CrushingGhostflame>().ToMutable(player),
            HexaghostModelDb.Ghostflame<BolsteringGhostflame>().ToMutable(player),
            HexaghostModelDb.Ghostflame<SearingGhostflame>().ToMutable(player),
            HexaghostModelDb.Ghostflame<CrushingGhostflame>().ToMutable(player),
            HexaghostModelDb.Ghostflame<InfernoGhostflame>().ToMutable(player)
        ] : [
            HexaghostModelDb.Ghostflame<OffclassSearingGhostflame>().ToMutable(player),
            HexaghostModelDb.Ghostflame<OffclassCrushingGhostflame>().ToMutable(player),
            HexaghostModelDb.Ghostflame<OffclassBolsteringGhostflame>().ToMutable(player),
            HexaghostModelDb.Ghostflame<OffclassSearingGhostflame>().ToMutable(player),
            HexaghostModelDb.Ghostflame<OffclassCrushingGhostflame>().ToMutable(player),
            HexaghostModelDb.Ghostflame<OffclassInfernoGhostflame>().ToMutable(player)
        ];
    }

    public override Task BeforeCombatStart()
    {
        var state = CombatManager.Instance.DebugOnlyGetState();
        if (state == null) return Task.CompletedTask;
        foreach (var player in state.Players)
        {
            ResetWheel(player);
            if (player.Character is Hexaghost) HexaghostCmd.ActivateGhostwheel(player);
            HexaghostCmd.Refresh(player);
        }

        return Task.CompletedTask;
    }

    public static void ResetWheel(Player player)
    {
        Wheel[player] = StartingWheel(player);
        CurrentIndex[player] = 0;
    }
    
    // we use AfterSideTurnEnd instead of BeforeSideTurnEnd so thermal stone triggers on cards that got ethereal exhausted
    // we have to care about order with HereAndNowPower
    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player) return;
        foreach (var player in RunManager.Instance.State?.Players ?? [])
        {
            if (HexaghostCmd.GetCurrentFlame(player).IsIgnited)
                await HexaghostCmd.Advance(ctx, player, null, true, true);
        }
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)
    {
        if ( card is not IHasAfterlifeEffect afterlifeEffect) return;
        await afterlifeEffect.AfterlifeEffect(ctx, null, true, causedByEthereal);
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