using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;

namespace Hermit.HermitCode.Relics;

/// <summary>
///     Whenever you defeat an Elite encounter, heal 7 HP and gain 35 gold.
/// </summary>
public sealed class BrokenTooth : HermitRelicModel
{
    public BrokenTooth() : base(RelicRarity.Rare)
    {
        WithHeal(7);
        WithGold(35);
    }

    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        if (room.RoomType != RoomType.Elite) return;
        Flash();
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        await PlayerCmd.GainGold(DynamicVars.Gold.BaseValue, Owner);
        
    }
}