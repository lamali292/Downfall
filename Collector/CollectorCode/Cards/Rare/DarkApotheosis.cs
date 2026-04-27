using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Piles;
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

    protected override Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        foreach (var cardModel in CollectorPile.Collected.GetPile(Owner).Cards) CardCmd.Upgrade(cardModel);
        return Task.CompletedTask;
    }
}