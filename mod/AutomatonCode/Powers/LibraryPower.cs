using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Powers;

public class LibraryPower : AutomatonPowerModel
{
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player != Owner.Player) return;
        var choice = AutomatonCmd.GetEncodableCards(player, Amount).Select(t =>
        {
            t.SetToFreeThisTurn();
            return t;
        });
        await CardPileCmd.AddGeneratedCardsToCombat(choice, PileType.Hand, player);
        Flash();
    }
}