using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Collector.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class Hoard : CollectorCardModel
{
    public Hoard() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPyre();
        WithCards(2, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var cards = await CommonActions.Draw(this, ctx);
        foreach (var card in cards)
            card.GiveSingleTurnRetain();
    }
}