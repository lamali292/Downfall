using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Commands;

public static class MyCommonActions
{
    public static async Task Apply<T>(CardModel card, CardPlay cardPlay) where T : PowerModel
    {
        switch (card)
        {
            case { TargetType: TargetType.AnyEnemy or TargetType.AnyAlly or TargetType.AnyPlayer }:
                if (cardPlay.Target is null) break;
                await CommonActions.Apply<T>(cardPlay.Target, card);
                break;
            case { TargetType: TargetType.AllEnemies, CombatState: not null }:
                await CommonActions.Apply<T>(card.CombatState.Enemies, card);
                break;
            case { TargetType: TargetType.RandomEnemy, CombatState: not null }:
                await CommonActions.Apply<T>(
                    card.CombatState.Enemies.TakeRandom(1, card.CombatState.RunState.Rng.CombatTargets), card);
                break;
        }
    }
}