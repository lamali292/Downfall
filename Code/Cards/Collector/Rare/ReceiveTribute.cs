using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Collector.Rare;

[Pool(typeof(CollectorCardPool))]
public class ReceiveTribute : CollectorCardModel
{
    // Todo: nah
    public ReceiveTribute() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithPyre();
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
    }
}