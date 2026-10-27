using BaseLib.Extensions;
using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Collectibles;

public class LagavulinMatriarchCard : Collectible<LagavulinMatriarchBoss>
{
    public LagavulinMatriarchCard() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self, 0.91f)
    {
        WithPower<LagavulinMatriarchCardPower>(3, false);
        // todo: we do plating now. idk if thats good conversion of numbers.
        WithPower<PlatingPower>(4, 2);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        (await CommonActions.ApplySelf<LagavulinMatriarchCardPower>(ctx, this))?
            .SetSecondAmount(DynamicVars.Power<PlatingPower>().BaseValue);
    }
}
