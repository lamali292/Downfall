using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class Misdirect : CollectorCardModel
{
    public Misdirect() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(2, 2);
        WithPower<ReserveNextTurnPower>(1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<ReserveNextTurnPower>(ctx, this);
    }
}