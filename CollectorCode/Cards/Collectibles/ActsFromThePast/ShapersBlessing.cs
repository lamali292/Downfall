using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Collectibles.ActsFromThePast;

public class ShapersBlessing : ActsFromThePastCard
{
    public ShapersBlessing() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self, "DONU_AND_DECA_BOSS")
    {
        WithPower<StrengthPower>(2, 1);
        WithPower<PlatingPower>(4, 2);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<StrengthPower>(ctx, this);
        await CommonActions.ApplySelf<PlatingPower>(ctx, this);
    }
}