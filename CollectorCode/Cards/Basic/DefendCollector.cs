using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Basic;

[Pool(typeof(CollectorCardPool))]
public class DefendCollector : CollectorCardModel
{
    public DefendCollector() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithTags(CardTag.Defend);
        WithBlock(5, 3);
    }

    protected override Artist Artist => Artist.Get<Opal>();


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }
}