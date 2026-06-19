using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Relics;

[Pool(typeof(SlimeBossRelicPool))]
public class TarrBlob : SlimeBossRelicModel
{
    public TarrBlob() : base(RelicRarity.Ancient)
    {
        WithEnergy(1);
        WithVar("Decrease", 1);
    }

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return player == Owner ? amount + DynamicVars.Energy.BaseValue : amount;
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (Owner.PlayerCombatState is not { TurnNumber: 1 } || player != Owner) return;
        await SlimeBossCmd.DecreaseSlots(ctx, player, DynamicVars["Decrease"].IntValue);
    }
}