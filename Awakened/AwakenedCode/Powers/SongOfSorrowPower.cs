using Awakened.AwakenedCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
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
        if (card is not Void || card.Owner != Owner.Player || LocalContext.NetId == null)
            return;

     

        Flash();

        var currentEnemies = CombatState.Enemies.ToList();
        var ctx = new HookPlayerChoiceContext(
            card.Owner,
            LocalContext.NetId.Value,
            GameActionType.Combat);
        foreach (var enemy in currentEnemies)
            if (enemy is { IsHittable: true, IsAlive: true })
            {
                var task = CreatureCmd.Damage(ctx,
                    enemy,
                    Amount,
                    ValueProp.Unblockable | ValueProp.Unpowered,
                    Owner);

                await ctx.AssignTaskAndWaitForPauseOrCompletion(task);
            }
                
    }
}