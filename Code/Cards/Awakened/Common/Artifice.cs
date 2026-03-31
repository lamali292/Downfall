using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Common;


[Pool(typeof(AwakenedCardPool))]
public class Artifice : AwakenedCardModel
{
    public Artifice() : base(2, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithPower<ManaburnPower>(7);
        WithKeywords(CardKeyword.Retain);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await MyCommonActions.Apply<ManaburnPower>(cardPlay.Target, this);
    }


    protected override void OnUpgrade()
    {
        DynamicVars.Power<ManaburnPower>().UpgradeValueBy(3);
    }
}