using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Interfaces;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class StashAway : CollectorCardModel, IHasPyre
{
    public StashAway() : base(0, CardType.Skill, CardRarity.Ancient, TargetType.Self)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithReserve(1);
    }

    protected override Artist Artist => Artist.Get<Opal>();


    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        await CommonActions.ApplySelf<ReserveNextTurnPower>(ctx, this, x + 2);
        if (!IsUpgraded) return;
        await CommonActions.ApplySelf<DrawCardsNextTurnPower>(ctx, this, x + 1);
    }

    public CardModel? PyredCard { get; set; }
}