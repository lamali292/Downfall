using Downfall.Code.Abstract;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Hexaghost;

public class LivingBombPower() : HexaghostPowerModel(PowerType.Debuff, PowerStackType.Single), IShouldSoulburnDetonateTargetAll, IAfterSoulburnDetonate
{
    public bool ShouldSoulburnDetonateTargetAll(PlayerChoiceContext ctx, Creature owner)
    {
        return owner == Owner;
    }

    public async Task AfterSoulburnDetonate(PlayerChoiceContext ctx, Creature creature)
    {
        if (creature != Owner) return;
        await PowerCmd.Remove(this);
    }
}