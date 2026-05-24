using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Events;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Automaton.AutomatonCode.Powers;

public class OptimizePower : AutomatonPowerModel, IModifyStashDraw
{
    public int ModifyStashDraw(int amount, Player player)
    {
        return player.Creature == Owner ? amount + Amount : amount;
    }
}