using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using AwakenedCharacter = Awakened.AwakenedCode.Core.Awakened;

namespace Awakened.AwakenedCode.Ftue;

public sealed class AwakenedFtueHooks() : CustomSingletonModel(HookType.Combat)
{
    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player is { Character: AwakenedCharacter } && LocalContext.IsMe(player)
            && player.PlayerCombatState is { TurnNumber: 1 })
        {
            AwakenedFtue.QueueSpellbookAndMeter(player);
        }
        return Task.CompletedTask;
    }
}
