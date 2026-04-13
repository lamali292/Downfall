using Downfall.Code.Abstract;
using Downfall.Code.Vfx;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Powers.Awakened;

public class DarkEchoPower: AwakenedPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task BeforeTurnEnd(PlayerChoiceContext ctx, CombatSide side)
    {
        if (side != Owner.Side) return;
        var damageAmount = Owner.GetPowerAmount<StrengthPower>() + 4;
        SfxPlayer.PlaySfx("res://Downfall/audio/awakened_one_3.ogg");
        for (var i = 0; i < Amount; i++)
        {
            var creatureNode = NCombatRoom.Instance?.GetCreatureNode(Owner);
            if (creatureNode != null)
            {
                var spawnPos = creatureNode.VfxSpawnPosition;
                var vfx1 = NShockWaveVfx.Create(spawnPos, new Color(0.1f, 0.0f, 0.2f, 1.0f));
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx1);
                await Cmd.Wait(0.1f);
                var vfx2 = NShockWaveVfx.Create(spawnPos, new Color(0.3f, 0.2f, 0.4f, 1.0f));
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx2);
           
       
            }
            await Cmd.Wait(0.5f);
            var enemies = CombatState.Enemies.ToList();
            foreach (var enemy in enemies.Where(e => e.IsAlive))
            {
                await CreatureCmd.Damage(ctx, enemy, damageAmount, ValueProp.Unpowered, Owner);
            }
        }
    }

    
  
}