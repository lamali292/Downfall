using Collector.CollectorCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Encounters;

namespace Collector.CollectorCode.Cards.Collectibles;


public class TestSubjectCard : Collectible<TestSubjectBoss>
{
    public TestSubjectCard() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.3f)
    {
    }
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        // Todo
    }
}
