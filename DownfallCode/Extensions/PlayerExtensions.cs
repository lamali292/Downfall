using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Extensions;

public static class PlayerExtensions
{
    extension(Player player)
    {
        public IReadOnlyList<Player> AllTeammates
            => player.Creature.CombatState!.GetTeammatesOf(player.Creature)
                .Where(e => e.IsAlive)
                .Select(c => c.Player)
                .OfType<Player>().ToArray();

        public IReadOnlyList<Player> OtherTeammates => player.AllTeammates.Where(p => p != player).ToArray();
        public Player? RandomOtherTeammate => player.RunState.Rng.CombatTargets.NextItem(player.OtherTeammates);

        public IReadOnlyList<CardModel> DeckPile => PileType.Deck.GetPile(player).Cards;
        public IReadOnlyList<CardModel> Hand => PileType.Hand.GetPile(player).Cards;
        public IReadOnlyList<CardModel> DiscardPile => PileType.Discard.GetPile(player).Cards;
        public IReadOnlyList<CardModel> DrawPile => PileType.Draw.GetPile(player).Cards;
        public IReadOnlyList<CardModel> ExhaustPile => PileType.Exhaust.GetPile(player).Cards;

        public IEnumerable<CardModel> GetAllCombatCards => player.PlayerCombatState?.AllCards ?? [];
    }
}