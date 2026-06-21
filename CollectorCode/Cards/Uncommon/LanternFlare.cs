using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Interfaces;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class LanternFlare : CollectorCardModel, IHasPyre
{
    public LanternFlare() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithPower<CollectorDoomPower>(12, 3);
        WithPower<ScorchedPower>(3, 1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public CardModel? PyredCard { get; set; }


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<CollectorDoomPower>(ctx, this, cardPlay);
        await CommonActions.Apply<ScorchedPower>(ctx, this, cardPlay);
    }
}