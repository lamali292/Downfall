using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Guardian;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class TimeSifter : GuardianCardModel
{
    public TimeSifter() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
        WithPower<TimeSifterPower>(1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<TimeSifterPower>(this);
    }
}