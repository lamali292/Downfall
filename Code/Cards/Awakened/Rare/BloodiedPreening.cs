using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.Awakened.Token;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class BloodiedPreening : AwakenedCardModel
{
    public BloodiedPreening() : base(0, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithTip(typeof(StrengthPower));
        WithTip(typeof(PlumeJab));
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<StrengthPower>(ctx, this, -2);
        await CommonActions.ApplySelf<BloodiedPreeningPower>(ctx, this, 1);
    }
}