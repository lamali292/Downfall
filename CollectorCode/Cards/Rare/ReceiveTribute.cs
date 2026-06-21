using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Interfaces;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class ReceiveTribute : CollectorCardModel, IHasPyre
{
    // Todo: nah
    public ReceiveTribute() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithKeyword(CardKeyword.Exhaust);
        WithCards(2, 1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public CardModel? PyredCard { get; set; }

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }
}