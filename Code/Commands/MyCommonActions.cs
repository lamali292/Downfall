using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Commands;

public static class MyCommonActions
{
    public static async Task Apply<T>(PlayerChoiceContext ctx, CardModel card, CardPlay? cardPlay) where T : PowerModel
    {
        if (cardPlay?.Target is null && card.TargetType == TargetType.AnyEnemy)
        {
            await ApplyToRandomEnemy<T>(ctx, card);
            return;
        }
        switch (card)
        {
            case { TargetType: TargetType.AnyEnemy or TargetType.AnyAlly or TargetType.AnyPlayer }:
                if (cardPlay is null) break;
                await ApplyToEnemy<T>(ctx, card, cardPlay);
                break;
            case { TargetType: TargetType.AllEnemies, CombatState: not null }:
                await ApplyToAllEnemies<T>(ctx, card);
                break;
            case { TargetType: TargetType.RandomEnemy, CombatState: not null }:
                await ApplyToRandomEnemy<T>(ctx, card);
                break;
        }
    }
    
    public static async Task ApplyToAllEnemies<T>(PlayerChoiceContext ctx, CardModel card) where T : PowerModel
    {
        if (card.CombatState == null) return;
        await CommonActions.Apply<T>(ctx, card.CombatState.HittableEnemies, card);
    }
    
    public static async Task ApplyToRandomEnemy<T>(PlayerChoiceContext ctx, CardModel card) where T : PowerModel
    {
        if (card.CombatState == null) return;
        await CommonActions.Apply<T>(ctx, card.CombatState.HittableEnemies.TakeRandom(1, card.CombatState.RunState.Rng.CombatTargets), card);
    }
    
    public static async Task ApplyToEnemy<T>(PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay) where T : PowerModel
    {
        if (cardPlay.Target is null) return;
        await CommonActions.Apply<T>(ctx, cardPlay.Target, card);
    }
}