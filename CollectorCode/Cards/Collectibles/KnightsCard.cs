using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Collectibles;

public class KnightsCard : Collectible<KnightsElite>
{
    public KnightsCard() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, 0.3f)
    {
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
        WithKeyword(CardKeyword.Ethereal);
        WithPower<MachineLearningPower>(2, false);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MachineLearningPower>(ctx, this);
        await CommonActions.ApplySelf<HexPower>(ctx, this, 1);
    }
}
