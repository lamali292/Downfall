using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Interfaces;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class FingerOfDeath : CollectorCardModel, IUsesCollectorEnergyOnly
{
    public FingerOfDeath() : base(4, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithPower<MiasmaPower>(50, 17);
        WithReserveTip();
    }

    protected override Artist Artist => Artist.Get<Opal>();
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<MiasmaPower>(ctx, this, cardPlay);
    }
}