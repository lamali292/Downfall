using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Collectibles;


public class TestSubjectCard : Collectible<TestSubjectBoss>
{
    public TestSubjectCard() : base(3, CardType.Power, CardRarity.Rare, TargetType.AnyEnemy, 0.3f)
    {
        WithCostUpgradeBy(-1);
        WithPower<EnragePower>(1, true);
        WithTip<StrengthPower>();
    }
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<EnragePower>(ctx, this);
    }
}
