using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Commands;

public static class MyCommonActions
{
    public static async Task<IReadOnlyList<T>> Apply<T>(PlayerChoiceContext ctx, CardModel card, CardPlay? cardPlay) where T : PowerModel
    {
        switch (card)
        {
            case { TargetType: TargetType.AnyEnemy or TargetType.AnyAlly or TargetType.AnyPlayer }:
                if (cardPlay is null) break;
                return await ApplyToEnemy<T>(ctx, card, cardPlay);
            case { TargetType: TargetType.AllEnemies, CombatState: not null }:
                return await ApplyToAllEnemies<T>(ctx, card);
            case { TargetType: TargetType.RandomEnemy, CombatState: not null }:
                return await ApplyToRandomEnemy<T>(ctx, card);
        }
        return [];
    }

    public static async Task<IReadOnlyList<T>> ApplyToAllEnemies<T>(PlayerChoiceContext ctx, CardModel card) where T : PowerModel
    {
        if (card.CombatState == null) return [];
        return await CommonActions.Apply<T>(ctx, card.CombatState.HittableEnemies, card);
    }


    public static async Task<IReadOnlyList<T>> ApplyToRandomEnemy<T>(PlayerChoiceContext ctx, CardModel card) where T : PowerModel
    {
        var enemy = card.CombatState?.HittableEnemies.TakeRandom(1, card.CombatState.RunState.Rng.CombatTargets)
            .FirstOrDefault();
        if (enemy == null) return [];
        var result =  await CommonActions.Apply<T>(ctx,enemy, card);
        return result == null ? [] : [result];
    }

    public static async Task<IReadOnlyList<T>> ApplyToEnemy<T>(PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay)
        where T : PowerModel
    {
        if (cardPlay.Target is null) return [];
        var result = await CommonActions.Apply<T>(ctx, cardPlay.Target, card);
        return result == null ? [] : [result];
    }

    public static async Task CardCalculatedBlock(CardModel card, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(card, card.DynamicVars.CalculatedBlock, cardPlay);
    }
}