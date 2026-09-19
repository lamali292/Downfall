using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Powers;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Encounters;
namespace Collector.CollectorCode.Cards.Collectibles;

public class VantomCard : Collectible<VantomBoss>
{
    public VantomCard() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self, 0.67f)
    {
        WithPower<PlayerSlipperyPower>(2);
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<PlayerSlipperyPower>(ctx, this);
    }
}
