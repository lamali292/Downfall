using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Guardian;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Rare;

[Pool(typeof(GuardianCardPool))]
public class StasisEngine : GuardianCardModel
{
    public StasisEngine() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithEnergyTip();
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
        WithPower<StasisEnginePower>(1);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<StasisEnginePower>(ctx, this);
    }
}