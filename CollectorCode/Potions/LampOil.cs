using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Potions;

[Pool(typeof(CollectorPotionPool))]
public class LampOil : CollectorPotionModel
{
    public LampOil() : base(PotionRarity.Rare, PotionUsage.CombatOnly, TargetType.AnyPlayer)
    {
        WithKindle(22);
    }

    protected override Artist Artist => Artist.Get<Fulgur>();
    
    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        if (target?.Player == null) return;
        await CollectorCmd.Kindle(ctx, target.Player, this);
    }
}