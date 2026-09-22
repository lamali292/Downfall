using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Encounters;

namespace Collector.CollectorCode.Cards.Collectibles;

public class KnightsCard : Collectible<KnightsElite>
{
    public KnightsCard() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self, 0.4f)
    {
        WithTip(CardKeyword.Ethereal);
        WithPower<MachineLearningPower>(2, false);
        WithPower<KnightsCardPower>(1, false);
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MachineLearningPower>(ctx, this);
        await CommonActions.ApplySelf<KnightsCardPower>(ctx, this);
    }
}
