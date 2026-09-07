using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Potions;

[Pool(typeof(CollectorPotionPool))]
public class OldTimeRations : CollectorPotionModel
{
    public OldTimeRations() : base(PotionRarity.Uncommon, PotionUsage.CombatOnly, TargetType.AnyPlayer)
    {
        WithReserve(3);
    }

    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        if (target?.Player == null) return;
        await CollectorCmd.GetReserve(target.Player, DynamicVars.Reserve.IntValue);
    }
   
}