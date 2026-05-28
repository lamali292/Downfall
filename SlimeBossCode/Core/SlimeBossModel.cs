using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Core;

public class SlimeBossModel() : CustomSingletonModel(HookType.Combat)
{
    private static IEnumerable<CustomMonsterModel>? _cllCustomMonsterModel;

    private static IEnumerable<CustomMonsterModel> AllCustomMonsterModel =>
        _cllCustomMonsterModel ??= ModelDb.AllAbstractModelSubtypes
            .Where(t => t.IsSubclassOf(typeof(MonsterModel)))
            .Select(t => (MonsterModel)ModelDb.Get(t))
            .OfType<CustomMonsterModel>()
            .ToList();

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player.Character is not SlimeBoss) return;
        var slimeModel = AllCustomMonsterModel.OfType<SlimeModel>().TakeRandom(1, combatState.RunState.Rng.Niche)
            .FirstOrDefault();
        if (slimeModel == null) return;
        await SlimeQueue.AddSlime(player, slimeModel);
    }
    
    public override Task BeforeCombatStart()
    {
        var state = CombatManager.Instance.DebugOnlyGetState();
        if (state == null) return Task.CompletedTask;
        SlimeQueue.ResetAllSlots();
        foreach (var player in state.Players.Where(e => e.Character is SlimeBoss))
        {
            SlimeQueue.SetSlots(player, 3);
        }
        return Task.CompletedTask;
    }
}