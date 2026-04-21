using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Piles;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Collector.Rare;

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