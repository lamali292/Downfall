using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Extensions;
public static class PlayerExtension
{
    extension(Player player)
    {
        public List<Creature> Slimes => GetSlimes(player);
        public int SlimeCount => player.Slimes.Count;
        public Creature? GetSlime<T>() where T : SlimeModel
        {
            return player.Creature.Pets.FirstOrDefault(e => e.Monster is T);
        }
    }
    
    private static List<Creature> GetSlimes(Player player)
    {
        return player.PlayerCombatState?.Pets.Where(e => e.Monster is SlimeModel).ToList() ?? [];
    }

}