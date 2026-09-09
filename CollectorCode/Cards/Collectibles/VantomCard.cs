using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;
namespace Collector.CollectorCode.Cards.Collectibles;

public class VantomCard : Collectible<VantomBoss>
{
    public VantomCard() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self, 0.3f)
    {
        WithPower<SlipperyPower>(1, 1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<SlipperyPower>(ctx, this);
    }
}
