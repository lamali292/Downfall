using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class GreenpyreLocus : CollectorCardModel
{
    public GreenpyreLocus() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, false, false)
    {
        WithCards(2, 1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        throw new NotImplementedException();
    }
}