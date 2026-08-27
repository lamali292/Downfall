using BaseLib.Utils;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Rare;

public class LongShadows : HermitCardModel
{
    public LongShadows() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<LongShadowsPower>(1, false);
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<LongShadowsPower>(ctx, this);
    }
}