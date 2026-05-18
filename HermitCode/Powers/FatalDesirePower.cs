using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Hermit.HermitCode.Powers;

public sealed class FatalDesirePower() : HermitPowerModel(PowerType.Debuff)
{
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;

        Flash();
        for (var i = 0; i < Amount; i++)
        {
            var injury = CombatState.CreateCard<Injury>(Owner.Player!);
            await CardPileCmd.AddGeneratedCardToCombat(
                injury,
                PileType.Hand,
                Owner.Player
            );
        }
    }
}