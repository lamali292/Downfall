using BaseLib.Utils;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Collectibles.Ruina2;

public class EternallyLitLamp : Ruina2Card
{
    public EternallyLitLamp() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self, "BIG_BIRD_ELITE", 0.61f)
    {
        WithCostUpgradeBy(-1);
        WithTip<StrengthPower>();
        WithPower<EternallyLitLampPower>(1, false);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<EternallyLitLampPower>(ctx, this);
    }
}