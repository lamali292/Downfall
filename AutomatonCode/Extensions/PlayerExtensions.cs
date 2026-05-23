using Automaton.AutomatonCode.Piles;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Extensions;

public static class PlayerExtensions
{
    public static IReadOnlyList<CardModel> GetStash(this Player player)
    {
        return StashPile.Stash.GetPile(player).Cards;
    }

    public static IReadOnlyList<CardModel> GetEncode(this Player player)
    {
        return EncodePile.FunctionSequence.GetPile(player).Cards;
    }
}