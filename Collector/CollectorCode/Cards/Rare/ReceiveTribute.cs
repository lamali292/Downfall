using BaseLib.Utils;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class ReceiveTribute : CollectorCardModel
{
    // Todo: nah
    public ReceiveTribute() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithPyre();
        WithKeyword(CardKeyword.Exhaust);
        WithCards(2, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
    }
}