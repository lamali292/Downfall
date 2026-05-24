using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class BloodiedPreening : AwakenedCardModel
{
    public BloodiedPreening() : base(0, CardType.Power, CardRarity.Rare, TargetType.Self)
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