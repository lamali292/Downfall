using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class Arrogance : CollectorCardModel
{
    public Arrogance() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithPower<DoomedAttackPower>(4, 2, false);
        WithTip<MiasmaPower>();
        WithBlock(8, 3);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<DoomedAttackPower>(ctx, this);
    }
}