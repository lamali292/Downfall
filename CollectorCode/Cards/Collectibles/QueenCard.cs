using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Collectibles;

public class QueenCard : Collectible<QueenBoss>
{
    public QueenCard() : base(3, CardType.Power, CardRarity.Rare, TargetType.AnyEnemy, 0.3f)
    {
        WithCostUpgradeBy(-1);
        WithKeyword(CollectorKeyword.Megapyre);
        WithPower<PyrePower>(3);
        WithPower<ChainsOfBindingPower>(3, -1);
        WithEnergy(3);
        WithCards(3);
    }
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<PyrePower>(ctx, this);
        await CommonActions.ApplySelf<ChainsOfBindingPower>(ctx, this);
    }
}
