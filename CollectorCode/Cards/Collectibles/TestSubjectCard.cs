using BaseLib.Abstracts;
using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Collectibles;


public class TestSubjectCard : Collectible<TestSubjectBoss>
{
    public TestSubjectCard() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self, 0.3f)
    {
        WithCostUpgradeBy(-1);
        WithTip<StrengthPower>();
        WithPower<TestSubjectCardPower>(1, false);
    }
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<TestSubjectCardPower>(ctx, this);
    }
}
