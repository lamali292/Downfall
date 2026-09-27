using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Extensions;
public static class PlayerExtension
{
    extension(Player player)
    {
        public List<Creature> SlimeCreatures => GetSlimes(player);
        public int SlimeCount => player.SlimeCreatures.Sum(e => (e.Monster as SlimeModel)?.SlimeAmount ?? 0);
        public Creature? GetSlime<T>() where T : SlimeModel
        {
            return player.Creature.Pets.FirstOrDefault(e => e.Monster is T);
        }
        public Creature? GetSlime(SlimeModel slimeModel)
        {
            return player.Creature.Pets.FirstOrDefault(e => e.Monster?.GetType() == slimeModel.GetType());
        }
    }
    
    private static List<Creature> GetSlimes(Player player)
    {
        return player.PlayerCombatState?.Pets.Where(e => e.Monster is SlimeModel).ToList() ?? [];
    }

}