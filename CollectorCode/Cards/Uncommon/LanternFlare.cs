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
public class LanternFlare : CollectorCardModel
{
    public LanternFlare() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithPower<MiasmaPower>(12, 3);
        WithPower<ScorchedPower>(2, 1, false);
    }

    protected override Artist Artist => Artist.Get<Opal>();


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<MiasmaPower>(ctx, this, cardPlay);
        await CommonActions.Apply<ScorchedPower>(ctx, this, cardPlay);
    }
}