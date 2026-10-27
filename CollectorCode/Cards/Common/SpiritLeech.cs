using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class SpiritLeech : CollectorCardModel
{
    public SpiritLeech() : base(2, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        WithTorchheadDamage(14, 6);//Yes these numbers are balanced.
        WithPower<ReserveNextTurnPower>(1, false);
        WithReserveTip();
        WithTip(CollectorKeyword.Torchhead);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (Owner.IsTorchheadAlive)
        {
        await TorchheadCmd.TorchheadAttack(this, cardPlay).ExecuteIfPresent(ctx);
        }
        else
        {
            await TorchheadCmd.Kindle(ctx, Owner, 1, this);
        }
        await CommonActions.ApplySelf<ReserveNextTurnPower>(ctx, this);
    }
    protected override bool ShouldGlowRedInternal => Owner.IsTorchheadMissing;
}