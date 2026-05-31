using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Piles;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class DarkApotheosis : CollectorCardModel
{
    public DarkApotheosis() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        foreach (var cardModel in CollectorPile.Collected.GetPile(Owner).Cards) CardCmd.Upgrade(cardModel);
        return Task.CompletedTask;
    }
}