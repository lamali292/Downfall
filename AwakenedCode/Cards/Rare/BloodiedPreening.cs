using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class BloodiedPreening : AwakenedCardModel
{
    public BloodiedPreening() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithTip<StrengthPower>();
        this.WithTip<PlumeJab>();
        WithVar("StrengthLoss", 2);
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<StrengthPower>(ctx, this, -DynamicVars["StrengthLoss"].BaseValue);
        await CommonActions.ApplySelf<BloodiedPreeningPower>(ctx, this, 1);
    }
}