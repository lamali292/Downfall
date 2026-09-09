using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Encounters;
namespace Collector.CollectorCode.Cards.Collectibles;

public class BygoneEffigyCard : Collectible<BygoneEffigyElite>
{
    public BygoneEffigyCard() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, 0.3f)
    {
        WithVar("Power", 3);
        WithPower<BygoneEffigyCardPower>(3, -1, false);
        WithReserveTip();
        WithTip<PlatedArmorPower>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        (await CommonActions.ApplySelf<BygoneEffigyCardPower>(ctx, this))?.SetEffect(DynamicVars["Power"].BaseValue);
    }
}
