using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Common;


[Pool(typeof(AwakenedCardPool))]
public class FromWithin : AwakenedCardModel
{
    public FromWithin() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(10);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<FromWithinPower>(this, 1);
    }


    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}