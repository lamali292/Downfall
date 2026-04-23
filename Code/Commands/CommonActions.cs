using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Commands;

public static class MyCommonActions
{
    public static async Task Apply<T>(CardModel card, CardPlay? cardPlay = null) where T : PowerModel
    {
        if (cardPlay?.Target is null && card.TargetType == TargetType.AnyEnemy)
        {
            await ApplyToRandomEnemy<T>(card);
            return;
        }
        switch (card)
        {
            case { TargetType: TargetType.AnyEnemy or TargetType.AnyAlly or TargetType.AnyPlayer }:
                if (cardPlay is null) break;
                await ApplyToEnemy<T>(card, cardPlay);
                break;
            case { TargetType: TargetType.AllEnemies, CombatState: not null }:
                await ApplyToAllEnemies<T>(card);
                break;
            case { TargetType: TargetType.RandomEnemy, CombatState: not null }:
                await ApplyToRandomEnemy<T>(card);
                break;
        }
    }
    
    public static async Task ApplyToAllEnemies<T>(CardModel card) where T : PowerModel
    {
        if (card.CombatState == null) return;
        await CommonActions.Apply<T>(card.CombatState.Enemies, card);
    }
    
    public static async Task ApplyToRandomEnemy<T>(CardModel card) where T : PowerModel
    {
        if (card.CombatState == null) return;
        await CommonActions.Apply<T>(card.CombatState.HittableEnemies.TakeRandom(1, card.CombatState.RunState.Rng.CombatTargets), card);
    }
    
    public static async Task ApplyToEnemy<T>(CardModel card, CardPlay cardPlay) where T : PowerModel
    {
        if (cardPlay.Target is null) return;
        await CommonActions.Apply<T>(cardPlay.Target, card);
    }
    
    
}