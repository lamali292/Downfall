using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Common;

[Pool(typeof(AwakenedCardPool))]
public class Artifice : AwakenedCardModel
{
    public Artifice() : base(2, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithPower<ManaburnPower>(7, 3);
        WithKeywords(CardKeyword.Retain);
    }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<ManaburnPower>(ctx, this, cardPlay);
    }
}