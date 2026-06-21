using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class ScorchingRay : CollectorCardModel
{
    public ScorchingRay() : base(0, CardType.Attack, CardRarity.Common, TargetType.RandomEnemy)
    {
        WithDamage(8, 3);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var amount = ResolveEnergyXValue();
        await CommonActions.CardAttack(this, cardPlay, amount).Execute(ctx);
    }
}