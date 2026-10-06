using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Extensions;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class ScorchingRay : CollectorCardModel
{
    public ScorchingRay() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithTorchheadDamage(10, 4);
        WithTip(CollectorKeyword.Torchhead);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var amount = ResolveEnergyXValue();
        for (var v = 0; v < amount; v++)
        {
            if (Owner.IsTorchheadAlive)
            {
            await TorchheadCmd.TorchheadAttack(this, cardPlay).ExecuteIfPresent(ctx);
            }
            else
            {
                await TorchheadCmd.Kindle(ctx, Owner, 1, this);
            }
        }
    }
    protected override bool ShouldGlowRedInternal => Owner.IsTorchheadMissing;
}