using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Relics;

/// <summary>
///     Prevent your first lethal HP loss each combat.
/// </summary>
[Obsolete]
public sealed class PetGhost() : HermitRelicModel(RelicRarity.Rare, false)
{
    private bool _usedThisCombat;

    private bool UsedThisCombat
    {
        get => _usedThisCombat;
        set
        {
            AssertMutable();
            _usedThisCombat = value;
        }
    }

    public override Task BeforeCombatStart()
    {
        Status = RelicStatus.Normal;
        UsedThisCombat = false;
        return Task.CompletedTask;
    }

    public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner.Creature || amount < Owner.Creature.CurrentHp || UsedThisCombat)
            return amount;

        Flash();
        Status = RelicStatus.Disabled;
        UsedThisCombat = true;

        return 0;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
}