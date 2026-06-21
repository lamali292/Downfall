using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Powers;

public class MergeConflictPower : AutomatonPowerModel
{
    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? player)
    {
        if (player?.Creature != Owner) return;
        var pile = card.Pile?.Type;
        if (pile == null) return;
        await PowerCmd.Decrement(this);
        Flash();
        var clone = card.CreateClone();
        var a = await CardPileCmd.AddGeneratedCardToCombat(clone, pile.Value, player);
        if (pile == PileType.Hand) return;
        CardCmd.PreviewCardPileAdd(a);
    }
}