using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Rare;

[Pool(typeof(GuardianCardPool))]
public class StasisEngine : GuardianCardModel
{
    public StasisEngine() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithEnergyTip();
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
        WithPower<StasisEnginePower>(1, false);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<StasisEnginePower>(ctx, this);
    }
}