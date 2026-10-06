using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using HexaghostCharacter = Hexaghost.HexaghostCode.Core.Hexaghost;

namespace Hexaghost.HexaghostCode.Ftue;

public sealed class HexaghostFtueHooks() : CustomSingletonModel(HookType.Combat)
{
    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player is { Character: HexaghostCharacter } && LocalContext.IsMe(player)
            && player.PlayerCombatState is { TurnNumber: 1 })
        {
            HexaghostFtue.QueueWheel(player);
        }
        return Task.CompletedTask;
    }
}
