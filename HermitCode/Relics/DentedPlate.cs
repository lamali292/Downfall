using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Hermit.HermitCode.Relics;

/// <summary>
///     While your HP is at or below 50%, gain Energy and draw 1 card at the start of your turn.
/// </summary>
[Obsolete]
public sealed class DentedPlate : HermitRelicModel
{
    public DentedPlate() : base(RelicRarity.Rare, false)
    {
        WithEnergy(1);
        WithCards(1);
    }

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != Owner || player.Creature.CurrentHp > player.Creature.MaxHp / 2)
            return count;
        return count + DynamicVars.Cards.BaseValue;
    }

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        if (player != Owner || player.Creature.CurrentHp > player.Creature.MaxHp / 2) return amount;
        return amount + DynamicVars.Energy.BaseValue;
        
    }
}