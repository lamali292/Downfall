using MegaCrit.Sts2.Core.Entities.Players;

namespace Automaton.AutomatonCode.Events;


public interface IModifyStashDraw
{
    int ModifyStashDraw(int amount, Player player);
}