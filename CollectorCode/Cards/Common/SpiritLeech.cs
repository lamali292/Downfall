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
    public SpiritLeech() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithTorchheadDamage(14, 6);//Yes these numbers are balanced.
        WithPower<ReserveNextTurnPower>(1, false);
        WithReserveTip();
        WithTip(CollectorKeyword.Torchhead);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    /// <summary>Torchhead hits what you pick; while it targets ALL enemies (Equip Axe) there is nothing to pick.</summary>
    public override TargetType TargetType => _owner != null && IsMutable && ShouldTorcheadTargetAll
        ? TargetType.AllEnemies
        : TargetType.AnyEnemy;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await TorchheadCmd.TorchheadAttack(this, cardPlay).ExecuteIfPresent(ctx);
        await CommonActions.ApplySelf<ReserveNextTurnPower>(ctx, this);
    }
}