using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class Pyrework : CollectorCardModel
{
    public Pyrework() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<PyreworkPower>(1, 1, false);
        WithReserveTip();
        WithTip(CardKeyword.Exhaust);
        WithTorchheadDamage(5);
    }

 
    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<PyreworkPower>(ctx, this);
    }
}