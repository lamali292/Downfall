using Automaton.AutomatonCode.Events;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;

namespace Automaton.AutomatonCode.Core;

public class AutomatonRunModel() : CustomSingletonModel(HookType.Run)
{
    public override Task AfterRoomEntered(AbstractRoom room)
    {
        var state = CombatManager.Instance.DebugOnlyGetState();
        var combatRoomNode = NCombatRoom.Instance;
        if (state == null || combatRoomNode == null) return Task.CompletedTask;
        /*foreach (var player in state.Players)
            if (player.Character is Automaton)
                NSequenceDisplay.SetupFor(combatRoomNode, player);*/
        return Task.CompletedTask;
    }
}

public class AutomatonCombatModel() : CustomSingletonModel(HookType.Combat)
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        var modified = AutomatonHook.ModifyStashDraw(combatState, 1, player, out _);
        await StashCmd.DrawFromStash(ctx, player, modified);
    }

    /// <summary>
    ///     Encodes the played card here rather than inside its OnPlay wrapper: the game runs
    ///     <c>Enchantment.OnPlay</c> after <c>CardModel.OnPlay</c>, so state such as Momentum's extra
    ///     damage is only final by now. Encoding earlier would snapshot the stale value into the Encode
    ///     pile preview and, for the last card, into the compiled Function.
    /// </summary>
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (AutomatonCmd.IsEncodable(cardPlay.Card))
            await AutomatonCmd.EncodeCard(cardPlay.Card, ctx);
    }
}