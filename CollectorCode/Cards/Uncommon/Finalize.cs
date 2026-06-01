using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class Finalize : CollectorCardModel
{
    public Finalize() : base(4, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithPower<CollectorDoomPower>(24, 4);
        WithPower<FinalizePower>(6, 2);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<CollectorDoomPower>(ctx, this, cardPlay);
        await CommonActions.Apply<FinalizePower>(ctx, this, cardPlay);
    }
}