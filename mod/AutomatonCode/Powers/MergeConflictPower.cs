using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Powers;

public class MergeConflictPower : AutomatonPowerModel, IAfterCompilingFunction
{
    public async Task AfterCompilingFunction(PlayerChoiceContext ctx, Player player, CardPileAddResult result)
    {
        if (player.Creature != Owner || result.cardAdded is not FunctionCard card) return;
        var pile = card.Pile?.Type ?? result.targetPile;
        await PowerCmd.Decrement(this);
        Flash();
        var clone = card.CreateClone();
        var a = await CardPileCmd.AddGeneratedCardToCombat(clone, pile, player);
        if (pile == PileType.Hand) return;
        CardCmd.PreviewCardPileAdd(a);
    }
}
