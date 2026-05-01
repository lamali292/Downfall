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
    protected override async Task AfterCardGeneratedForCombat(PlayerChoiceContext ctx, CardModel card, Player? player)
    {
        if (card is not Void || card.Owner != Owner.Player || LocalContext.NetId == null)
            return;
        Flash();
        var currentEnemies = CombatState.Enemies.ToList();
        foreach (var enemy in currentEnemies)
            if (enemy is { IsHittable: true, IsAlive: true })
                await CreatureCmd.Damage(ctx,
                    enemy,
                    Amount,
                    ValueProp.Unblockable | ValueProp.Unpowered,
                    Owner);
    }
}