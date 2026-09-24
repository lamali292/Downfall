using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class StashAway : CollectorCardModel
{
    public StashAway() : base(0, CardType.Skill, CardRarity.Ancient, TargetType.Self)
    {
        WithBlock(11, 1);
        WithKeyword(CollectorKeyword.Pyre);
        WithPower<ReserveNextTurnPower>(1, 1, false);
        WithPower<DrawCardsNextTurnPower>(2, false);
        //WithCards(2);
        //WithReserve(1);
        WithReserveTip();
        //WithKeyword(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<Opal>();
    
    //protected override bool HasEnergyCostX => false;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<ReserveNextTurnPower>(ctx, this);
        await CommonActions.ApplySelf<DrawCardsNextTurnPower>(ctx, this);
    }
}