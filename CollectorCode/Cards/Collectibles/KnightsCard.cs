using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Downfall.DownfallCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Encounters;

namespace Collector.CollectorCode.Cards.Collectibles;

public class KnightsCard : Collectible<KnightsElite>
{
    public KnightsCard() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, 0.4f)
    {//Todo: See if players like draw or power version more.
        WithTip(CardKeyword.Ethereal);
        //WithPower<MachineLearningPower>(2, false);
        //WithPower<KnightsCardPower>(1, false);
        WithCards(3, 1);
        WithKeywords(CardKeyword.Ethereal);
        //WithKeyword(CardKeyword.Innate, UpgradeType.Add);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        //await CommonActions.ApplySelf<MachineLearningPower>(ctx, this);
        //await CommonActions.ApplySelf<KnightsCardPower>(ctx, this);
        //At the start of your turn, draw {MachineLearningPower:diff()} additional {MachineLearningPower:plural:card|cards}.
        //ALL your cards are Ethereal.
        
        var cards = (await CommonActions.Draw(this, ctx)).ToList();
        TempKeywordUtil.Add(cards, CardKeyword.Ethereal, TempKeywordRemoveCondition.StartOfEnemyTurn);
    }
}
