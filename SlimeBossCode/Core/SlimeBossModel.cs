using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Core;

public class SlimeBossModel() : CustomSingletonModel(true, true)
{
    
    private static IEnumerable<CustomMonsterModel>? _allCustomModifier;
    private static IEnumerable<CustomMonsterModel> AllCustomModifier => 
        _allCustomModifier ??= ModelDb.AllAbstractModelSubtypes
            .Where(t => t.IsSubclassOf(typeof(MonsterModel)))
            .Select(t => (MonsterModel)ModelDb.Get(t))
            .OfType<CustomMonsterModel>()
            .ToList();
    
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player.Character is not SlimeBoss) return;
        var slimeModel = AllCustomModifier.OfType<SlimeModel>().TakeRandom(1, combatState.RunState.Rng.Niche).FirstOrDefault();
        if (slimeModel == null) return;
        var pet = player.Creature.CombatState?.CreateCreature(slimeModel.ToMutable(), player.Creature.Side, null);
        if (pet == null) return;
        await PlayerCmd.AddPet(pet, player);
    }

 
}