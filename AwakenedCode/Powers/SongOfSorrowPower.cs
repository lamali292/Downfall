using Awakened.AwakenedCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Awakened.AwakenedCode.Powers;

public class SongOfSorrowPower : AwakenedPowerModel
{
    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? player)
    {
        if (card is not Void || player?.Creature != Owner)
            return;
        var ctx = new BlockingPlayerChoiceContext();
        Flash();
        var currentEnemies = CombatState.Enemies.ToList();
        foreach (var enemy in currentEnemies)
            if (enemy is { IsHittable: true, IsAlive: true })
                await CreatureCmd.Damage(ctx,
                    enemy,
                    Amount,
                    DamageProps.nonCardHpLoss,
                    Owner);
    }
}