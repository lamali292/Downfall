using BaseLib.Extensions;
using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Encounters;


namespace Collector.CollectorCode.Cards.Collectibles;

public class LagavulinMatriarchCard : Collectible<LagavulinMatriarchBoss>
{
    public LagavulinMatriarchCard() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self, 0.3f)
    {
        WithPower<LagavulinMatriarchCardPower>(3, false);
        WithPower<PlatedArmorPower>(4, 2);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        (await CommonActions.ApplySelf<LagavulinMatriarchCardPower>(ctx, this))?
            .SetSecondAmount(DynamicVars.Power<PlatedArmorPower>().BaseValue);
    }
}
