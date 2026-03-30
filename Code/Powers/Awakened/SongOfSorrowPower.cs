using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Downfall.Code.Powers.Awakened;

public class SongOfSorrowPower : AwakenedPowerModel 
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;


    public override async Task AfterCardGeneratedForCombat(CardModel card, bool addedByPlayer)
    {
        if (card is not Void || card.Owner != Owner.Player || LocalContext.NetId == null) 
            return;
    
        var ctx = new HookPlayerChoiceContext(
            card.Owner,
            LocalContext.NetId.Value,
            GameActionType.Combat);

        Flash();
        
        var currentEnemies = CombatState.Enemies.ToList(); 

        foreach (var enemy in currentEnemies)
        {
            if (enemy is { IsHittable: true, IsAlive: true })
            {
                await CreatureCmd.Damage(ctx,
                    enemy, 
                    Amount,
                    ValueProp.Unblockable | ValueProp.Unpowered,
                    Owner);
            }
        }
    }
}