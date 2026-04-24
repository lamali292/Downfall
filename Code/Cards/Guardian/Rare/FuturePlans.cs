using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Guardian;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Rare;

[Pool(typeof(GuardianCardPool))]
public class FuturePlans : GuardianCardModel
{
    public FuturePlans() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<FuturePlansPower>(1);
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<FuturePlansPower>(ctx, this);
    }
}