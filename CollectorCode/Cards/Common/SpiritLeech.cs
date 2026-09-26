using BaseLib.Utils;
using Collector.CollectorCode.Core;
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
        WithTorchheadDamage(15, 6);
        WithPower<ReserveNextTurnPower>(1, false);
        WithReserveTip();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CollectorCmd.TorchheadAttack(this, cardPlay).ExecuteIfPresent(ctx);
        await CommonActions.ApplySelf<ReserveNextTurnPower>(ctx, this);
    }
}