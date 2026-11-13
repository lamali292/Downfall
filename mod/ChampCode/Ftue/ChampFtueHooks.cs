using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using ChampCharacter = Champ.ChampCode.Core.Champ;

namespace Champ.ChampCode.Ftue;

public sealed class ChampFtueHooks() : CustomSingletonModel(HookType.Combat)
{
    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player is { Character: ChampCharacter } && LocalContext.IsMe(player)
            && player.PlayerCombatState is { TurnNumber: 1 })
        {
            ChampFtue.QueueRules(player);
        }
        return Task.CompletedTask;
    }
}
